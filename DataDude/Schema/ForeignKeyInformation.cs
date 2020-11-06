namespace DataDude.Schema
{
    public class ForeignKeyInformation
    {
        public ForeignKeyInformation(string name, TableInformation table, ColumnInformation column)
        {
            Name = name;
            Table = table;
            Column = column;
        }
        public string Name { get; }
        public TableInformation Table { get; }
        public ColumnInformation Column { get; }
    }
}
