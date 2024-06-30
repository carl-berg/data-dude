using System.Data;

namespace DataDude.Instructions.Insert
{
    public class ColumnValue
    {
        public ColumnValue(object value)
        {
            Type = ColumnValueType.Set;
            Value = value;
        }

        private ColumnValue(ColumnValueType type)
        {
            Type = type;
        }

        public static ColumnValue Ignore => new ColumnValue(ColumnValueType.Ignore);
        public static ColumnValue NotSet => new ColumnValue(ColumnValueType.NotSet);
        public object? Value { get; private set; }
        public ColumnValueType Type { get; private set; }
        public DbType? DbType { get; set; }
        public static ColumnValue Null(DbType dbType) => new ColumnValue(ColumnValueType.Set) { DbType = dbType };
        public void Set(ColumnValue newValue)
        {
            Value = newValue.Value;
            Type = newValue.Type;
            DbType = newValue.DbType;
        }

        public void Set(object newValue) => Set(new ColumnValue(newValue));
    }
}
