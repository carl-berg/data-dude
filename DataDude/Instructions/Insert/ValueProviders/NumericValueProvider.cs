﻿using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public class NumericValueProvider : ValueProvider
    {
        protected override ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value)
        {
            if (column.DataType is "int" or "smallint" or "tinyint" or "bigint" or "decimal" or "numeric" or "money" or "smallmoney" or "real" or "float")
            {
                return new ColumnValue(0);
            }

            return null;
        }
    }
}
