namespace DataDude.Handlers
{
    public class DataDudeInstructionHandlerResult
    {
        public DataDudeInstructionHandlerResult(bool handled) => Handled = handled;
        public bool Handled { get; }
    }
}
