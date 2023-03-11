using System;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class GuidValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "uniqueidentifier")
            {
                return new ColumnValue(Guid.NewGuid());
            }

            return null;
        }
    }
}
