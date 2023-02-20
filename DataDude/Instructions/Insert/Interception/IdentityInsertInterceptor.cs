using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Core;

namespace DataDude.Instructions.Insert.Interception
{
    public class IdentityInsertInterceptor : IInsertInterceptor
    {
        public async ValueTask OnInsert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null)
        {
            if (statement.Data.Any(x => x.Value.Type == ColumnValueType.Set && x.Column.IsIdentity))
            {
                await connection.ExecuteAsync($"SET IDENTITY_INSERT {statement.Table.FullName} ON", transaction: transaction);
            }
        }

        public async ValueTask OnInserted(InsertedRow insertedRow, InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null)
        {
            if (statement.Data.Any(x => x.Value.Type == ColumnValueType.Set && x.Column.IsIdentity))
            {
                await connection.ExecuteAsync($"SET IDENTITY_INSERT {statement.Table.FullName} OFF", transaction: transaction);
            }
        }
    }
}
