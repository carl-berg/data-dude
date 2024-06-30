using DataDude.Schema;

namespace DataDude.Instructions
{
    /// <summary>
    /// Attempting to speed up DataDude by statically caching and pre-loading sql schema and calculated dependencies in between runs
    /// Warning: This is an experimental feature
    /// </summary>
    internal class StaticCache : IInstructionDecorator
    {
        protected static SchemaInformation? Schema { get; set; }
        protected static Dictionary<string, IDictionary<TableInformation, IReadOnlyList<TableInformation>>> Dependencies { get; set; } = [];

        public ValueTask Initialize(DataDudeContext context)
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
            return default;
        }

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
