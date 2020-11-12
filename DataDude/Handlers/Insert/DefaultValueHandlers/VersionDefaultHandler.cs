using DataDude.Schema;

namespace DataDude.Handlers.Insert.DefaultValueHandlers
{
    public class VersionDefaultHandler : DefaultValueHandler
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
