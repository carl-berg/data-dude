using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class StringValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "varchar" or "nvarchar" or "text")
            {
                return new ColumnValue(string.Empty);
            }

            return null;
        }
    }
}
