namespace DataDude.Handlers.Insert
{
    public class ColumnValue
    {
        private ColumnValue(ColumnValueType type, object? value = null)
        {
            Type = type;
            Value = value;
        }

        public object? Value { get; }

        public ColumnValueType Type { get; }

        public static ColumnValue Set(object value) => new ColumnValue(ColumnValueType.Set, value);
        public static ColumnValue Ignore() => new ColumnValue(ColumnValueType.Ignore);
        public static ColumnValue NotSet() => new ColumnValue(ColumnValueType.NotSet);
    }
}
