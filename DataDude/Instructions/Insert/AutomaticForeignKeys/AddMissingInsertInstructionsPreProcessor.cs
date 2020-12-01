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
            var toInsert = new Dictionary<InsertInstruction, IEnumerable<TableInformation>>();
            foreach (var instruction in context.Instructions.OfType<InsertInstruction>())
            {
                if (context.Schema?[instruction.TableName] is { } table)
                {
                    var dependencies = service.GetOrderedDependenciesFor(table)
                        .Where(t => !toInsert.Values.SelectMany(x => x).Contains(t))
                        .ToList();

                    if (dependencies.Any())
                    {
                        toInsert.Add(instruction, dependencies);
                    }
                }
            }

            foreach (var instruction in toInsert.Keys)
            {
                var index = context.Instructions.IndexOf(instruction);
                foreach (var table in toInsert[instruction].Reverse())
                {
                    context.Instructions.Insert(index, new InsertInstruction(table.FullName));
                }
            }

            return Task.CompletedTask;
        }
    }
}
