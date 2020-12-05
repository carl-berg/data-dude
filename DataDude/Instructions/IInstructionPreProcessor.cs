using System.Threading.Tasks;

namespace DataDude.Instructions
{
    public interface IInstructionPreProcessor
    {
        Task PreProcess(DataDudeContext context);
    }
}
