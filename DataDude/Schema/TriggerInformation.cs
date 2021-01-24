namespace DataDude.Schema
{
    public class TriggerInformation
    {
        public TriggerInformation(TableInformation table, string name, bool isDisabled)
        {
            Table = table;
            Name = name;
            IsDisabled = isDisabled;
        }

        public TableInformation Table { get; }
        public string Name { get; }
        public bool IsDisabled { get; }
    }
}
