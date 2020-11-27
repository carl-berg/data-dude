namespace DataDude.Schema
{
    public class ColumnInformation
    {
        public ColumnInformation(
            TableInformation table,
            string name,
            string dataType,
            bool isPrimaryKey,
            bool isIdentity,
            bool isNullable,
            bool isComputed,
            string? defaultValue,
            int maxLength,
            int precision,
            int scale)
        {
            Table = table;
            Name = name;
            DataType = dataType;
            IsPrimaryKey = isPrimaryKey;
            IsIdentity = isIdentity;
            IsNullable = isNullable;
            IsComputed = isComputed;
            DefaultValue = defaultValue;
            MaxLength = maxLength;
            Precision = precision;
            Scale = scale;
        }

        public TableInformation Table { get; }
        public string Name { get; }
        public string FullName => $"{Table.FullName}.{Name}";
        public string DataType { get; } = default!;
        public bool IsPrimaryKey { get; }
        public bool IsIdentity { get; }
        public bool IsNullable { get; }
        public bool IsComputed { get; }
        public string? DefaultValue { get; }
        public int MaxLength { get; }
        public int Precision { get; }
        public int Scale { get; }
    }
}
