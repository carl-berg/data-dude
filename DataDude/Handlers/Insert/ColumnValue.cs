namespace DataDude.Handlers.Insert
{
    public class ColumnValue
    {
        public ColumnValue(object value)
        {
            Type = ColumnValueType.Set;
            Value = value;
        }

        private ColumnValue(ColumnValueType type, object? value = null)
        {
            Type = type;
            Value = value;
        }

        public object? Value { get; private set; }

        public ColumnValueType Type { get; private set; }

        public static ColumnValue Ignore() => new ColumnValue(ColumnValueType.Ignore);
        public static ColumnValue NotSet() => new ColumnValue(ColumnValueType.NotSet);

        public void Set(ColumnValue value)
        {
            Value = value.Value;
            Type = value.Type;
        }
    }
}
