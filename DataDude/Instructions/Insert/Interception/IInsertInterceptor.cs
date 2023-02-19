using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Interception
{
    public interface IInsertInterceptor
    {
        Task OnInsert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null);
        Task OnInserted(InsertedRow insertedRow, InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null);
    }
}
