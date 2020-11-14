using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class NumericValueProvider : DefaultValueProvider
    {
        protected override ColumnValue? GetDefaultValue(TableInformation table, ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "int" or "smallint" or "bigint" or "decimal" or "numeric")
            {
                return new ColumnValue(0);
            }

            return null;
        }
    }
}
