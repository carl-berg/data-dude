using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Core;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.Insertion
{
    /// <summary>
    /// Insert handler that can handle an insert where not all primary keys have been set but can be generated.
    /// </summary>
    public class GeneratingInsertRowHandler : RowInsertHandler
    {
        public GeneratingInsertRowHandler(UniqueValueGenerator uniqueValueGenerator)
        {
            PrimaryKeyValueGenerator = uniqueValueGenerator;
        }

        public UniqueValueGenerator PrimaryKeyValueGenerator { get; set; }

        public override bool CanHandleInsert(InsertStatement statement, InsertContext context)
        {
            if (statement.Table.Any(column => column.IsPrimaryKey()))
            {
                return statement.Data
                    .Where(x => x.Column.IsPrimaryKey())
                    .All(x => CanHandleInsertOfPKColumn(x.Column, x.Value, context));
            }

            return false;
        }

        public override async ValueTask<InsertedRow> Insert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null)
        {
            await PreProcessStatement(statement, context, connection, transaction);
            var (columns, values, parameters) = GetInsertInformation(statement);
            var identityFilters = statement.Table.Where(x => x.IsPrimaryKey()).Select(x => $"[{x.Name}] = {GetParameterNameOrRawSql(x, statement.Data[x])}");
            var identityFilter = string.Join(" AND ", identityFilters);
            var insertedRow = await connection.QuerySingleAsync(
                $@"INSERT INTO {statement.Table.FullName}({columns}) VALUES({values})
                SELECT * FROM {statement.Table.FullName} WHERE {identityFilter}",
                parameters,
                transaction);

            return MakeInsertedRow(statement, insertedRow);
        }

        protected virtual IEnumerable<(string SQL, string ColumnName)> GetDatabaseGenratedValuePairs(InsertStatement statement)
        {
            foreach (var (column, value) in statement.Data.Where(x => x.Column.IsPrimaryKey()))
            {
                if (value.Value is RawSql rawSql)
                {
                    yield return (rawSql.ToString(), column.Name);
                }
                else if (value.Type == ColumnValueType.NotSet && column.DefaultValue is { } defaultSql)
                {
                    if (!defaultSql.Contains("newsequentialid") /* Cannot be used in a select statement */)
                    {
                        yield return (defaultSql, column.Name);
                    }
                }
            }
        }

        protected virtual bool CanHandleInsertOfPKColumn(ColumnInformation column, ColumnValue value, InsertContext context)
        {
            if (value.Type == ColumnValueType.Set)
            {
                return true;
            }
            else if (column.Table.ForeignKeys.Any(x => x.Columns.Any(x => x.Column == column)))
            {
                // Don't attempt to generate keys for FK columns
                return false;
            }

            return column.DefaultValue is { } || PrimaryKeyValueGenerator.CanHandle(column);
        }

        protected virtual async ValueTask PreProcessStatement(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null)
        {
            // Attempt to use specified raw-sql values or default values
            if (GetDatabaseGenratedValuePairs(statement) is { } fetch && fetch.Count() > 0)
            {
                var items = string.Join(", ", fetch.Select(f => $"{f.SQL} AS [{f.ColumnName}]"));
                var result = await connection.QuerySingleAsync($"SELECT {items}", transaction: transaction);

                foreach (var item in result)
                {
                    if (statement.Table[item.Key] is { } column)
                    {
                        statement.Data[column].Set(new ColumnValue(item.Value));
                    }
                }
            }

            // Attempt to generate new id's using unique value generator
            foreach (var (column, value) in statement.Data.Where(x => x.Column.IsPrimaryKey() && x.Value.Type == ColumnValueType.NotSet))
            {
                var generatedValue = PrimaryKeyValueGenerator.GenerateValue(context, column);
                statement.Data[column].Set(new ColumnValue(generatedValue));
            }
        }
    }
}
