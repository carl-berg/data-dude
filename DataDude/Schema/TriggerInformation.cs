namespace DataDude.Schema
{
    public class TriggerInformation
    {
        public TriggerInformation(string name, bool isDisabled)
        {
            Name = name;
            IsDisabled = isDisabled;
        }

        public string Name { get; }
        public bool IsDisabled { get; }
    }
}
