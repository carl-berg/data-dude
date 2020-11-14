namespace DataDude.Instructions
{
    public class HandleInstructionResult
    {
        public HandleInstructionResult(bool handled) => Handled = handled;
        public bool Handled { get; }
    }
}
