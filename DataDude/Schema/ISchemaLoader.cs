using System.Data;
using System.Threading.Tasks;
using DataDude.Schema;

namespace DataDude
{
    public interface ISchemaLoader
    {
        Task<SchemaInformation> Load(IDbConnection connection, IDbTransaction? transaction = null);
    }
}