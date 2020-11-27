using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class BinaryValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "varbinary")
            {
                return new ColumnValue(new byte[0]);
            }

            return null;
        }
    }
}
