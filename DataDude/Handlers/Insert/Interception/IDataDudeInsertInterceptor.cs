using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataDude.Handlers.Insert.Interception
{
    public interface IDataDudeInsertInterceptor
    {
        Task OnInsert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null);
        Task OnInserted(IDictionary<string, object> insertedRow, InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null);
    }
}
