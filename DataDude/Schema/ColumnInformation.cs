namespace DataDude.Schema
{
    public class ColumnInformation
    {
        public ColumnInformation(
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

        public string Name { get; }
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
