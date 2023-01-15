using System.Linq;

namespace DataDude.Schema
{
    public class ColumnInformation
    {
        private bool? _isPrimaryKey;

        public ColumnInformation(
            TableInformation table,
            string name,
            string dataType,
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
        public string DataType { get; } = default!;
        public bool IsIdentity { get; }
        public bool IsNullable { get; }
        public bool IsComputed { get; }
        public string? DefaultValue { get; }
        public int MaxLength { get; }
        public int Precision { get; }
        public int Scale { get; }
        public bool IsPrimaryKey()
        {
            return _isPrimaryKey ??= Table.Indexes
                .Where(x => x.IsPrimaryKey)
                .Any(index => index.Columns.Contains(this));
        }
    }
}
