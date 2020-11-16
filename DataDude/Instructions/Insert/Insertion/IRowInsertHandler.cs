using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Insertion
{
    public interface IRowInsertHandler
    {
        Task<IDictionary<string, object>> Insert(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null);
        Task<bool> CanHandleInsert(InsertStatement statement);
    }
}
