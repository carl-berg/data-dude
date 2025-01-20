using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class StringValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value)
        {
            if (column.IsText())
            {
                return new ColumnValue(string.Empty);
            }

            return null;
        }
    }
}
