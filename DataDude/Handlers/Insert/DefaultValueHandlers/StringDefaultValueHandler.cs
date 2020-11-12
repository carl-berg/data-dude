using DataDude.Schema;

namespace DataDude.Handlers.Insert.DefaultValueHandlers
{
    public class StringDefaultValueHandler : DefaultValueHandler
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
