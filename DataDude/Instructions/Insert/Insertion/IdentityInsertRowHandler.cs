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
    public class IdentityInsertRowHandler : RowInsertHandler
    {
        private readonly UniqueValueGenerator _uniqueValueGenerator;
        public IdentityInsertRowHandler(UniqueValueGenerator uniqueValueGenerator) => _uniqueValueGenerator = uniqueValueGenerator;

        public override bool CanHandleInsert(InsertStatement statement) => statement.Data
            .Where(x => x.Column.IsPrimaryKey)
            .All(x => CanHandleInsertOfPKColumn(statement.Table, x.Column, x.Value));

        public override async Task PreProcessStatement(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null)
        {
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

            foreach (var (column, value) in statement.Data.Where(x => x.Column.IsPrimaryKey && x.Value.Type == ColumnValueType.NotSet))
            {
                var generatedValue = _uniqueValueGenerator.GenerateValue(statement.Table, column);
                statement.Data[column].Set(new ColumnValue(generatedValue));
            }
        }

        public override async Task<InsertedRow> Insert(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var (columns, values, parameters) = GetInsertInformation(statement);
            var filters = statement.Table.Where(x => x.IsPrimaryKey).Select(x => $"{x.Name} = {GetParameterNameOrRawSql(x, statement.Data[x])}");
            var filter = string.Join(" AND ", filters);
            var insertedRow = await connection.QuerySingleAsync<dynamic>(
                $@"INSERT INTO {statement.Table.Schema}.{statement.Table.Name}({columns}) VALUES({values})
                SELECT * FROM {statement.Table.Schema}.{statement.Table.Name} WHERE {filter}",
                parameters,
                transaction);

            if (insertedRow is IReadOnlyDictionary<string, object> { } typedRow)
            {
                return new InsertedRow(typedRow, this);
            }
            else
            {
                throw new HandlerException("Could not parse inserted row as dictionary");
            }
        }

        private IEnumerable<(string SQL, string ColumnName)> GetDatabaseGenratedValuePairs(InsertStatement statement)
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

        private bool CanHandleInsertOfPKColumn(TableInformation table, ColumnInformation column, ColumnValue value)
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

            return column.DefaultValue is { } || _uniqueValueGenerator.CanHandle(table, column);
        }
    }
}
