using DataDude.Instructions.Insert.ValueProviders;
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

        public void InvokeValueProvider(IValueProvider provider)
        {
            foreach (var (column, value) in Data)
            {
                provider.Process(column, value);
            }
        }
    }
}
