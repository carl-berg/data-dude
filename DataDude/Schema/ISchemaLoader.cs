using System.Data;
using System.Threading.Tasks;
using DataDude.Schema;

namespace DataDude
{
    public interface ISchemaLoader
    {
        bool CacheSchema { get; set; }
        Task<SchemaInformation> Load(IDbConnection connection, IDbTransaction? transaction = null);
    }
}