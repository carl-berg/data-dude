using DataDude.Schema;

namespace DataDude.Instructions.Insert
{
    public interface IInsertValueProvider
    {
        void Process(TableInformation table, ColumnInformation column, ColumnValue previousValue);
    }
}
