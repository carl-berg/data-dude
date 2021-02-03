using System;
using System.Data;
using System.Linq;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public abstract class ValueProvider : IValueProvider
    {
        public void Process(ColumnInformation column, ColumnValue value)
        {
            if (column.DefaultValue is { Length: > 0 } ||
                column.IsPrimaryKey() ||
                column.IsIdentity ||
                column.IsComputed ||
                column.Table.ForeignKeys.Any(fk => fk.Columns.Any(fk_col => fk_col.Column == column)) ||
                value.Type is ColumnValueType.Ignore or ColumnValueType.Set)
            {
                return;
            }
            else if (column.IsNullable)
            {
                value.Set(ColumnValue.Null(DataDudeContext.GetDbType(column)));
            }
            else if (GetDefaultValue(column, value) is { } newValue)
            {
                value.Set(newValue);
            }
        }

        protected abstract ColumnValue? GetDefaultValue(ColumnInformation column, ColumnValue value);
    }
}
