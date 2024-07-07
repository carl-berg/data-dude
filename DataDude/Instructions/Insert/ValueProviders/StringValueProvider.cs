using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class StringValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "char" or "varchar" or "nchar" or "ntext" or "nvarchar" or "text")
            {
                return new ColumnValue(string.Empty);
            }

            return null;
        }
    }
}
