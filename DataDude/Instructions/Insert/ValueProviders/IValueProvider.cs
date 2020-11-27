using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public interface IValueProvider
    {
        void Process(ColumnInformation column, ColumnValue previousValue);
    }
}
