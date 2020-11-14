using DataDude.Schema;

namespace DataDude.Instructions.Insert
{
    public class InsertStatement
    {
        public InsertStatement(TableInformation table, InsertInstruction instruction)
        {
            Table = table;
            Data = new InsertData(table, instruction);
        }

        public TableInformation Table { get; }
        public InsertData Data { get; }

        public void InvokeValueProvider(IInsertValueProvider provider)
        {
            foreach (var (column, value) in Data)
            {
                provider.Process(Table, column, value);
            }
        }
    }
}
