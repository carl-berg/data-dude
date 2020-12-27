using System.Data;
using System.Threading.Tasks;

namespace DataDude.Schema
{
    public class CachableSchemaLoader : ISchemaLoader
    {
        private readonly ISchemaLoader _loader;
        private SchemaInformation? _cachedSchema;
        public CachableSchemaLoader(ISchemaLoader loader) => _loader = loader;

        public bool CacheSchema
        {
            get => _loader.CacheSchema;
            set => _loader.CacheSchema = value;
        }

        public async Task<SchemaInformation> Load(IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (CacheSchema)
            {
                return _cachedSchema ??= await _loader.Load(connection, transaction);
            }

            return await _loader.Load(connection, transaction);
        }
    }
}
