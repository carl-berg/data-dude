using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Interception
{
    public interface IInsertInterceptor
    {
        ValueTask OnInsert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null);
        ValueTask OnInserted(InsertedRow insertedRow, InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null);
    }
}
