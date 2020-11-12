using DataDude.Schema;

namespace DataDude.Handlers.Insert.DefaultValueHandlers
{
    public class BoolDefaultHandler : DefaultValueHandler
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
