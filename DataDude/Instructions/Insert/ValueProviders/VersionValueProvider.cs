using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class VersionValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(TableInformation table, ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "timestamp")
            {
                return ColumnValue.Ignore;
            }

            return null;
        }
    }
}
