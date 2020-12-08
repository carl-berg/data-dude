using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.AutomaticForeignKeys
{
    /// <summary>
    /// Adds "missing" dependent inserts based on foreign keys.
    /// </summary>
    public class AddMissingInsertInstructionsPreProcessor : IInstructionPreProcessor
    {
        public Task PreProcess(DataDudeContext context)
        {
            var service = new DependencyService();
            var toInsert = new Dictionary<InsertInstruction, InsertInformation>();
            foreach (var instruction in context.Instructions.OfType<InsertInstruction>())
            {
                if (context.Schema?[instruction.TableName] is { } table)
                {
                    var dependencies = service.GetOrderedDependenciesFor(table)
                        .Where(t => !toInsert.Values.Any(x => x.Contains(t)))
                        .ToList();

                    toInsert.Add(instruction, new InsertInformation(table, dependencies));
                }
            }

            foreach (var instruction in toInsert.Keys)
            {
                var index = context.Instructions.IndexOf(instruction);
                foreach (var table in toInsert[instruction].Dependencies.Reverse())
                {
                    context.Instructions.Insert(index, new InsertInstruction(table.FullName));
                }
            }

            return Task.CompletedTask;
        }

        private class InsertInformation
        {
            public InsertInformation(TableInformation table, IEnumerable<TableInformation> dependencies)
            {
                Table = table;
                Dependencies = dependencies;
            }

            public TableInformation Table { get; }
            public IEnumerable<TableInformation> Dependencies { get; }
            public bool Contains(TableInformation table)
            {
                return table == Table || Dependencies.Contains(table);
            }
        }
    }
}
