using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataDude.Handlers.Insert
{
    public interface IDataDudeInsertInterceptor
    {
        Task OnInsert(InsertStatement statement, DataDudeContext context);
        Task OnInserted(IDictionary<string, object> insertedRow, InsertStatement statement, DataDudeContext context);
    }
}
