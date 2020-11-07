using System.Collections.Generic;

namespace DataDude.Schema
{
    public class ColumnInformation
    {
        public ColumnInformation(
            string name,
            string dataType,
            bool isIdentity,
            bool isNullable,
            bool isComputed,
            bool hasDefaultValue,
            int maxLength,
            int precision,
            int scale)
        {
            Name = name;
            DataType = dataType;
            IsIdentity = isIdentity;
            IsNullable = isNullable;
            IsComputed = isComputed;
            HasDefaultValue = hasDefaultValue;
            MaxLength = maxLength;
            Precision = precision;
            Scale = scale;
        }

        public string Name { get; }
        public string DataType { get; } = default!;
        public bool IsIdentity { get; }
        public bool IsNullable { get; }
        public bool IsComputed { get; }
        public bool HasDefaultValue { get; }
        public int MaxLength { get; }
        public int Precision { get; }
        public int Scale { get; }
    }
}
