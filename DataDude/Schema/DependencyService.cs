using System.Collections.Generic;
using System.Linq;

namespace DataDude.Schema
{
    public class DependencyService
    {
        private readonly IDependencyTraversalStrategy _strategy;
        private readonly IDictionary<TableInformation, IReadOnlyList<TableInformation>> _cachedDependencies;

        public DependencyService(IDependencyTraversalStrategy strategy, DataDudeContext? context = null)
        {
            _strategy = strategy;
            _cachedDependencies = GetCache(context);
        }

        public IEnumerable<TableInformation> GetOrderedDependenciesFor(TableInformation table)
        {
            if (_cachedDependencies.TryGetValue(table, out var cachedDependencies))
            {
                return cachedDependencies;
            }

            var dependencies = new HashSet<TableInformation>();
            foreach (var dep in table.ForeignKeys.Where(_strategy.Process).Select(x => x.ReferencedTable))
            {
                ProcessDependencies(dep, ref dependencies);
            }

            var sortedDependencies = new List<TableInformation>();
            var dependenciesToAdd = dependencies.Where(x => x.ForeignKeys.Where(_strategy.Process).Count() == 0);
            while (dependenciesToAdd.Count() > 0)
            {
                sortedDependencies.AddRange(dependenciesToAdd);
                dependenciesToAdd = dependencies
                    .Except(sortedDependencies)
                    .Where(x => x.ForeignKeys.Where(_strategy.Process).All(fk => sortedDependencies.Contains(fk.ReferencedTable)));
            }

            if (sortedDependencies.Count() != dependencies.Count())
            {
                throw new DependencyTraversalFailedException($"Failed building a dependency chain for table {table.FullName}");
            }

            _cachedDependencies[table] = sortedDependencies;
            return sortedDependencies;
        }

        private void ProcessDependencies(TableInformation table, ref HashSet<TableInformation> dependencies)
        {
            dependencies.Add(table);
            foreach (var dep in table.ForeignKeys.Where(_strategy.Process).Select(x => x.ReferencedTable))
            {
                if (!dependencies.Contains(dep))
                {
                    ProcessDependencies(dep, ref dependencies);
                }
            }
        }

        private IDictionary<TableInformation, IReadOnlyList<TableInformation>> GetCache(DataDudeContext? context)
        {
            if (context?.Get<IDictionary<TableInformation, IReadOnlyList<TableInformation>>>("DependencyService_Cache") is IDictionary<TableInformation, IReadOnlyList<TableInformation>> cache)
            {
                return cache;
            }

            var newCache = new Dictionary<TableInformation, IReadOnlyList<TableInformation>>();
            context?.Set("DependencyService_Cache", newCache);
            return newCache;
        }
    }
}
