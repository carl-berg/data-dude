using DataDude.Schema;

namespace DataDude.Instructions.Insert.AutomaticForeignKeys
{
    /// <summary>
    /// Adds "missing" dependent inserts based on foreign keys.
    /// </summary>
    public class AddMissingInsertInstructionsPreProcessor(DependencyService dependencyService) : IInstructionDecorator
    {
        public ValueTask PreProcess(DataDudeContext context)
        {
            var toInsert = new Dictionary<InsertInstruction, InsertInformation>();
            foreach (var instruction in context.Instructions.OfType<InsertInstruction>())
            {
                if (context.Schema?[instruction.TableName] is { } table)
                {
                    IEnumerable<InsertedRow> insertedRows = InsertContext.Get(context)?.InsertedRows ?? Array.Empty<InsertedRow>();
                    var dependencies = dependencyService.GetOrderedDependenciesFor(table)
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

        private class InsertInformation(TableInformation table, IEnumerable<TableInformation> dependencies)
        {
            public TableInformation Table { get; } = table;
            public IEnumerable<TableInformation> Dependencies { get; } = dependencies;
            public bool Contains(TableInformation table)
            {
                return table == Table || Dependencies.Contains(table);
            }
        }
    }
}
