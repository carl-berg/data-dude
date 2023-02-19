using System.Data.Common;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Insertion
{
    public interface IInsertRowHandler
    {
        Task<InsertedRow> Insert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null);
        bool CanHandleInsert(InsertStatement statement, InsertContext context);
    }
}
