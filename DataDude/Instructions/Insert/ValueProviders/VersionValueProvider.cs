using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class VersionValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "timestamp" or "rowversion")
            {
                return ColumnValue.Ignore;
            }

            return null;
        }
    }
}
