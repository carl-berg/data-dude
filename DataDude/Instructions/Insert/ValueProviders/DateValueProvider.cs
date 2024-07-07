using System;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class DateValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value)
        {
            return column.DataType switch
            {
                "date" or "datetime" or "datetime2" => new ColumnValue(new DateTime(1753, 1, 1, 12, 0, 0)),
                "smalldatetime" => new ColumnValue(new DateTime(1900, 1, 1, 12, 0, 0)),
                "datetimeoffset" => new ColumnValue(new DateTimeOffset(1753, 1, 1, 12, 0, 0, TimeSpan.Zero)),
                "time" => new ColumnValue(TimeSpan.Zero),
                _ => null
            };
        }
    }
}
