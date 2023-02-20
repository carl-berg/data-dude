using System.Threading.Tasks;

namespace DataDude.Instructions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInstructionDecorator
    {
        ValueTask PreProcess(DataDudeContext context);
        ValueTask PostProcess(DataDudeContext context);
    }
}
