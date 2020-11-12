using DataDude.Schema;

namespace DataDude.Handlers.Insert.DefaultValueHandlers
{
    public class BinaryDefaultHandler : DefaultValueHandler
    {
        protected override ColumnValue? GetDefaultValue(TableInformation table, ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "varbinary")
            {
                return new ColumnValue(new byte[0]);
            }

            return null;
        }
    }
}
