using System.Threading.Tasks;

namespace DataDude.Instructions
{
    /// <summary>
    /// Intercept context before and after instructions have been executed
    /// </summary>
    public interface IInstructionDecorator
    {
        ValueTask PreProcess(DataDudeContext context);
        ValueTask PostProcess(DataDudeContext context);
    }
}
