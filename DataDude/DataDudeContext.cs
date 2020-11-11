using System.Collections.Generic;
using DataDude.Handlers;
using DataDude.Instructions;

namespace DataDude
{
    public class DataDudeContext
    {
        public IList<IDataDudeInstruction> Instructions { get; } = new List<IDataDudeInstruction>();
        public IList<IDataDudeInstructionHandler> InstructionHandlers { get; } = new List<IDataDudeInstructionHandler>
        {
            new ExecuteInstructionHandler(),
        };
    }
}
