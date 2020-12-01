using System;
using System.Collections.Generic;
using System.Linq;

namespace DataDude.Schema
{
    public class DependencyService
    {
        public IEnumerable<TableInformation> GetOrderedDependenciesFor(TableInformation table)
        {
            var dependencies = new HashSet<TableInformation>();
            foreach (var dep in table.ForeignKeys.Select(x => x.ReferencedTable))
            {
                ProcessDependencies(dep, ref dependencies);
            }

            var sortedDependencies = new List<TableInformation>();
            var dependenciesToAdd = dependencies.Where(x => x.ForeignKeys.Count() == 0);
            while (dependenciesToAdd.Count() > 0)
            {
                sortedDependencies.AddRange(dependenciesToAdd);
                dependenciesToAdd = dependencies
                    .Except(sortedDependencies)
                    .Where(x => x.ForeignKeys.All(fk => sortedDependencies.Contains(fk.ReferencedTable)));
            }

            if (sortedDependencies.Count() != dependencies.Count())
            {
                throw new Exception($"Failed building a dependency chain for table {table.FullName}");
            }

            return sortedDependencies;
        }

        private void ProcessDependencies(TableInformation table, ref HashSet<TableInformation> dependencies)
        {
            dependencies.Add(table);
            foreach (var dep in table.ForeignKeys.Select(x => x.ReferencedTable))
            {
                if (!dependencies.Contains(dep))
                {
                    ProcessDependencies(dep, ref dependencies);
                }
            }
        }
    }
}
