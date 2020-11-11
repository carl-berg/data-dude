using System.Data;
using System.Threading.Tasks;
using DataDude.Instructions;

namespace DataDude.Handlers
{
    public interface IDataDudeInstructionHandler
    {
        Task<DataDudeInstructionHandlerResult> Handle(IDataDudeInstruction instruction, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null);
    }
}
