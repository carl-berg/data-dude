using System.Data.Common;
using System.Threading.Tasks;
using DataDude.Schema;

namespace DataDude
{
    public interface ISchemaLoader
    {
        ValueTask<SchemaInformation> Load(DbConnection connection, DbTransaction? transaction = null);
    }
}