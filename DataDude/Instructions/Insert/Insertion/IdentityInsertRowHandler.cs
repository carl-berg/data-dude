using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Core;

namespace DataDude.Instructions.Insert.Insertion
{
    /// <summary>
    /// Insert handler using scope identity if possible to select inserted row.
    /// </summary>
    public class IdentityInsertRowHandler : RowInsertHandler
    {
        public override bool CanHandleInsert(InsertStatement statement, InsertContext context)
        {
            var primaryKeys = statement.Table.Where(x => x.IsPrimaryKey());
            return primaryKeys.Count() == 1 && primaryKeys.All(x => x.IsIdentity);
        }

        public override async ValueTask<InsertedRow> Insert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null)
        {
            var (columns, values, parameters) = GetInsertInformation(statement);
            var primaryKey = statement.Table.Single(x => x.IsPrimaryKey() && x.IsIdentity);
            var insertedRow = await connection.QuerySingleAsync(
                $@"INSERT INTO {statement.Table.FullName}({columns}) VALUES({values})
                SELECT * FROM {statement.Table.FullName} WHERE [{primaryKey.Name}] = SCOPE_IDENTITY()",
                parameters,
                transaction);

            return MakeInsertedRow(statement, insertedRow);
        }
    }
}
