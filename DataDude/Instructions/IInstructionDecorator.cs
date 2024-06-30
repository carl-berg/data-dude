namespace DataDude.Instructions
{
    /// <summary>
    /// Intercept context before and after instructions have been executed
    /// </summary>
    public interface IInstructionDecorator
    {
        /// <summary>
        /// Initialize decorator if needed. This is run before schema is loaded.
        /// </summary>
        ValueTask Initialize(DataDudeContext context) => default;

        /// <summary>
        /// Pre processing. This is run after schema has been loaded but before instructions are processed.
        /// </summary>
        ValueTask PreProcess(DataDudeContext context) => default;


        /// <summary>
        /// Post processing. This is run after instructions are processed.
        /// </summary>
        ValueTask PostProcess(DataDudeContext context) => default;
    }
}
