using System.Data;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Insertion
{
    public interface IInsertRowHandler
    {
        Task PreProcessStatement(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null);
        Task<InsertedRow> Insert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null);
        bool CanHandleInsert(InsertStatement statement, DataDudeContext context);
    }
}
