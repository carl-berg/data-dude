using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataDude.Instructions
{
    public interface IInstructionHandler
    {
        ValueTask<HandleInstructionResult> Handle(IInstruction instruction, DataDudeContext context, DbConnection connection, DbTransaction? transaction = null);
    }
}
