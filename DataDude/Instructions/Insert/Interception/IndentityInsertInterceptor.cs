using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Instructions.Insert.Insertion;

namespace DataDude.Instructions.Insert.Interception
{
    public class IndentityInsertInterceptor : IInsertInterceptor
    {
        public async Task OnInsert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            await connection.ExecuteAsync($"SET IDENTITY_INSERT {statement.Table.Schema}.{statement.Table.Name} ON", transaction: transaction);
        }

        public async Task OnInserted(InsertedRow insertedRow, InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            await connection.ExecuteAsync($"SET IDENTITY_INSERT {statement.Table.Schema}.{statement.Table.Name} OFF", transaction: transaction);
        }

        public virtual bool ShouldBeInvoked(InsertStatement statement, IInsertRowHandler handler) 
            => statement.Data.Any(x => x.Value.Type == ColumnValueType.Set && x.Column.IsIdentity);
    }
}
