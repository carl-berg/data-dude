using System.Data;
using System.Threading.Tasks;

namespace DataDude.Instructions
{
    public interface IInstructionHandler
    {
        Task<HandleInstructionResult> Handle(IInstruction instruction, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null);

        Task PreProcess(IInstruction instruction, DataDudeContext context);
    }
}
