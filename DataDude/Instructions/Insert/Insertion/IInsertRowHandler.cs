using System.Data;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Insertion
{
    public interface IInsertRowHandler
    {
        Task<InsertedRow> Insert(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null);
        bool CanHandleInsert(InsertStatement statement);
    }
}
