namespace DataDude.Instructions.Insert
{
    public class InsertInstruction : IInstruction
    {
        public InsertInstruction(string tableName, object? columnData = null)
        {
            TableName = tableName;
            ColumnValues = new ColumnValues(columnData);
        }

        public string TableName { get; }
        public ColumnValues ColumnValues { get; }
    }
}
