using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class BoolValueProvider : DefaultValueProvider
    {
        protected override ColumnValue? GetDefaultValue(TableInformation table, ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "bit")
            {
                return new ColumnValue(false);
            }

            return null;
        }
    }
}
