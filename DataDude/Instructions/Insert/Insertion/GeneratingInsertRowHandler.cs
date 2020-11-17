using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.Insertion
{
    /// <summary>
    /// Insert handler that can handle an insert where not all primary keys have been set but can be generated.
    /// </summary>
    public class GeneratingInsertRowHandler : RowInsertHandler
    {
        public override bool CanHandleInsert(InsertStatement statement, DataDudeContext context) => statement.Data
            .Where(x => x.Column.IsPrimaryKey)
            .All(x => CanHandleInsertOfPKColumn(statement.Table, x.Column, x.Value, context));

        public override async Task PreProcessStatement(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            // Attempt to use specified raw-sql values or default values
            if (GetDatabaseGenratedValuePairs(statement) is { } fetch && fetch.Count() > 0)
            {
                var items = string.Join(", ", fetch.Select(f => $"{f.SQL} AS {f.ColumnName}"));
                var result = await connection.QuerySingleAsync<dynamic>($"SELECT {items}", transaction: transaction);
                if (result is IReadOnlyDictionary<string, object> values)
                {
                    foreach (var item in values)
                    {
                        if (statement.Table[item.Key] is { } column)
                        {
                            statement.Data[column].Set(new ColumnValue(item.Value));
                        }
                    }
                }
            }

            // Attempt to generate new id's based on previously inserted rows
            if (context.InsertedRows.Where(x => x.Table == statement.Table).LastOrDefault() is { } lastInsert)
            {
                foreach (var (column, value) in statement.Data.Where(x => x.Column.IsPrimaryKey && x.Value.Type == ColumnValueType.NotSet))
                {
                    if (lastInsert?[column.Name] is int intValue)
                    {
                        statement.Data[column].Set(new ColumnValue(intValue + 1));
                    }
                    else if (lastInsert?[column.Name] is long longValue)
                    {
                        statement.Data[column].Set(new ColumnValue(longValue + 1));
                    }
                    else if (lastInsert?[column.Name] is short shortValue)
                    {
                        statement.Data[column].Set(new ColumnValue(shortValue + 1));
                    }
                }
            }

            // Attempt to generate new id's using unique value generator
            foreach (var (column, value) in statement.Data.Where(x => x.Column.IsPrimaryKey && x.Value.Type == ColumnValueType.NotSet))
            {
                var generatedValue = context.PrimaryKeyValueGenerator.GenerateValue(statement.Table, column);
                statement.Data[column].Set(new ColumnValue(generatedValue));
            }
        }

        public override async Task<InsertedRow> Insert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var (columns, values, parameters) = GetInsertInformation(statement);
            var identityFilters = statement.Table.Where(x => x.IsPrimaryKey).Select(x => $"{x.Name} = {GetParameterNameOrRawSql(x, statement.Data[x])}");
            var identityFilter = string.Join(" AND ", identityFilters);
            var insertedRow = await connection.QuerySingleAsync<dynamic>(
                $@"INSERT INTO {statement.Table.Schema}.{statement.Table.Name}({columns}) VALUES({values})
                SELECT * FROM {statement.Table.Schema}.{statement.Table.Name} WHERE {identityFilter}",
                parameters,
                transaction);

            if (insertedRow is IReadOnlyDictionary<string, object> { } typedRow)
            {
                return new InsertedRow(statement.Table, typedRow, this);
            }
            else
            {
                throw new InsertHandlerException("Could not parse inserted row as dictionary", statement: statement);
            }
        }

        protected virtual IEnumerable<(string SQL, string ColumnName)> GetDatabaseGenratedValuePairs(InsertStatement statement)
        {
            foreach (var (column, value) in statement.Data.Where(x => x.Column.IsPrimaryKey))
            {
                if (value.Value is RawSql rawSql)
                {
                    yield return (rawSql.ToString(), column.Name);
                }
                else if (value.Type == ColumnValueType.NotSet && column.DefaultValue is { } defaultSql)
                {
                    yield return (defaultSql, column.Name);
                }
            }
        }

        protected virtual bool CanHandleInsertOfPKColumn(TableInformation table, ColumnInformation column, ColumnValue value, DataDudeContext context)
        {
            if (value.Type == ColumnValueType.Set)
            {
                return true;
            }
            else if (table.ForeignKeys.Any(x => x.Columns.Any(x => x.Column == column)))
            {
                // Don't attempt to generate keys for FK columns
                return false;
            }

            return column.DefaultValue is { } || context.PrimaryKeyValueGenerator.CanHandle(table, column);
        }
    }
}
