using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DataDude.Instructions.Insert.Interception
{
    public class IndentityInsertInterceptor : IInsertInterceptor
    {
        public async Task OnInsert(InsertStatement statement, InsertContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (statement.Data.Any(x => x.Value.Type == ColumnValueType.Set && x.Column.IsIdentity))
            {
                await connection.ExecuteAsync($"SET IDENTITY_INSERT {statement.Table.FullName} ON", transaction: transaction);
            }
        }

        public async Task OnInserted(InsertedRow insertedRow, InsertStatement statement, InsertContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (statement.Data.Any(x => x.Value.Type == ColumnValueType.Set && x.Column.IsIdentity))
            {
                await connection.ExecuteAsync($"SET IDENTITY_INSERT {statement.Table.FullName} OFF", transaction: transaction);
            }
        }
    }
}
