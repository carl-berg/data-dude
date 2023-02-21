using System.Collections.Generic;
using System.Linq;
using DataDude.Schema;
using System.Threading.Tasks;

namespace DataDude.Instructions
{
    /// <summary>
    /// Attempting to speed up DataDude by statically caching and pre-loading sql schema and calculated dependencies in between runs
    /// Warning: This is an experimental feature
    /// </summary>
    public class StaticCache : IInstructionDecorator
    {
        public StaticCache(DataDudeContext context)
        {
            if (Schema is { })
            {
                // Pre-load schema
                context.Set(DataDudeContext.SchemaKey, Schema);
            }

            foreach (var (cacheKey, dependencyCache) in Dependencies)
            {
                // Pre-load calculated dependencies
                context.Set(cacheKey, dependencyCache);
            }
        }

        protected static SchemaInformation? Schema { get; set; }
        protected static Dictionary<string, IDictionary<TableInformation, IReadOnlyList<TableInformation>>> Dependencies { get; set; } = new();

        public ValueTask PreProcess(DataDudeContext context) => default;

        public ValueTask PostProcess(DataDudeContext context)
        {
            if (Schema is null)
            {
                // Cache loaded schema
                Schema = context.Schema;
            }

            // Cache calculated dependencies
            foreach (var cacheKey in context.ContextKeys.Where(key => key.StartsWith(DependencyService.CachePrefix) && Dependencies.ContainsKey(key) is false))
            {
                if (context.Get<IDictionary<TableInformation, IReadOnlyList<TableInformation>>>(cacheKey) is { } createdCache)
                {
                    Dependencies[cacheKey] = createdCache;
                }
            }

            return default;
        }
    }
}
