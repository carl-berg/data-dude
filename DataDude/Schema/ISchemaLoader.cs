using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using DataDude.Schema;

namespace DataDude
{
    public interface ISchemaLoader
    {
        Task<SchemaInformation> Load(DbConnection connection, DbTransaction? transaction = null);
    }
}