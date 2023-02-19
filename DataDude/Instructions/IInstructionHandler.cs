using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataDude.Instructions
{
    public interface IInstructionHandler
    {
        Task<HandleInstructionResult> Handle(IInstruction instruction, DataDudeContext context, DbConnection connection, DbTransaction? transaction = null);
    }
}
