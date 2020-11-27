using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class StringValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(TableInformation table, ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "varchar" or "nvarchar")
            {
                return new ColumnValue(string.Empty);
            }

            return null;
        }
    }
}
