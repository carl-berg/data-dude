﻿using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class BinaryValueProvider : DefaultValueProvider
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