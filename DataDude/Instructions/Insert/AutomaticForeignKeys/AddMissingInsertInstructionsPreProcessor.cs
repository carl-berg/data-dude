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
    public class AddMissingInsertInstructionsPreProcessor : IInstructionDecorator
    {
        private readonly DependencyService _dependencyService;
        public AddMissingInsertInstructionsPreProcessor(DependencyService dependencyService) => _dependencyService = dependencyService;

        public ValueTask PreProcess(DataDudeContext context)
        {
            var toInsert = new Dictionary<InsertInstruction, InsertInformation>();
            foreach (var instruction in context.Instructions.OfType<InsertInstruction>())
            {
                if (context.Schema?[instruction.TableName] is { } table)
                {
                    IEnumerable<InsertedRow> insertedRows = InsertContext.Get(context)?.InsertedRows ?? Array.Empty<InsertedRow>();
                    var dependencies = _dependencyService.GetOrderedDependenciesFor(table)
                        .Where(t => !toInsert.Values.Any(x => x.Contains(t)))
                        .Where(t => !insertedRows.Any(x => x.Table == t))
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

            return default;
        }

        public ValueTask PostProcess(DataDudeContext context) => default;

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
