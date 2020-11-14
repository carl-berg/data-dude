using System;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class DateValueProvider : DefaultValueProvider
    {
        protected override ColumnValue? GetDefaultValue(TableInformation table, ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "date" or "datetime" or "datetime2")
            {
                return new ColumnValue(new DateTime(1753, 1, 1, 12, 0, 0));
            }
            else if (column.DataType is "datetimeoffset")
            {
                return new ColumnValue(new DateTimeOffset(1753, 1, 1, 12, 0, 0, TimeSpan.Zero));
            }

            return null;
        }
    }
}
