using DataDude.Schema;

namespace DataDude.Handlers.Insert.DefaultValueHandlers
{
    public abstract class DefaultValueHandler : IDataDudeInsertValueHandler
    {
        public ColumnValue Handle(TableInformation table, ColumnInformation column, ColumnValue value)
        {
            if (column.IsNullable ||
                column.HasDefaultValue ||
                column.IsComputed ||
                column.IsIdentity ||
                value.Type is ColumnValueType.Ignore or ColumnValueType.Set)
            {
                return value;
            }
            else if (GetDefaultValue(table, column, value) is { } newValue)
            {
                return newValue;
            }

            return value;
        }

        protected abstract ColumnValue? GetDefaultValue(TableInformation table, ColumnInformation column, ColumnValue value);
    }
}
