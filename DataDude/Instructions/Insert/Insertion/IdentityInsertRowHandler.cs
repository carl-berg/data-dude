using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DataDude.Instructions.Insert.Insertion
{
    /// <summary>
    /// Insert handler that can handle an insert where all primary keys already are set (but not with rawsql).
    /// </summary>
    public class IdentityInsertRowHandler : RowInsertHandler
    {
        public override bool CanHandleInsert(InsertStatement statement)
        {
            var primaryKeys = statement.Table.Where(x => x.IsPrimaryKey);
            var allPksHaveBeenSet = primaryKeys.All(column => statement.Data[column].Type == ColumnValueType.Set && statement.Data[column].Value is not RawSql);
            return primaryKeys.Any() && allPksHaveBeenSet;
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
    }
}
