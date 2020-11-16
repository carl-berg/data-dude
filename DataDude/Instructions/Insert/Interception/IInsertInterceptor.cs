using System.Data;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Interception
{
    public interface IInsertInterceptor
    {
        Task OnInsert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null);
        Task OnInserted(InsertedRow insertedRow, InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null);
    }
}
