using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DataDude.Instructions.Insert.Insertion
{
    /// <summary>
    /// Insert handler using scope identity if possible to select inserted row.
    /// </summary>
    public class IdentityInsertRowHandler : RowInsertHandler
    {
        public override bool CanHandleInsert(InsertStatement statement, InsertContext context)
        {
            var primaryKeys = statement.Table.Where(x => x.IsPrimaryKey);
            return primaryKeys.Count() == 1 && primaryKeys.All(x => x.IsIdentity);
        }

        public override async Task<InsertedRow> Insert(InsertStatement statement, InsertContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var (columns, values, parameters) = GetInsertInformation(statement);
            var primaryKey = statement.Table.Single(x => x.IsPrimaryKey && x.IsIdentity);
            var insertedRow = await connection.QuerySingleAsync<object>(
                $@"INSERT INTO {statement.Table.Schema}.{statement.Table.Name}({columns}) VALUES({values})
                SELECT * FROM {statement.Table.Schema}.{statement.Table.Name} WHERE {primaryKey.Name} = SCOPE_IDENTITY()",
                parameters,
                transaction);

            return MakeInsertedRow(statement, insertedRow);
        }
    }
}
