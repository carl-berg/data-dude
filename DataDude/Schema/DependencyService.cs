using System.Collections.Generic;
using System.Linq;

namespace DataDude.Schema
{
    public class DependencyService
    {
        private readonly IDependencyTraversalStrategy _strategy;
        public DependencyService(IDependencyTraversalStrategy strategy) => _strategy = strategy;

        public IEnumerable<TableInformation> GetOrderedDependenciesFor(TableInformation table)
        {
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
    }
}
