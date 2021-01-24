using System.Collections.Generic;
using System.Diagnostics;

namespace DataDude.Schema
{
    [DebuggerDisplay("{Name}")]
    public class IndexInformation
    {
        public IndexInformation(TableInformation table, string name, IEnumerable<ColumnInformation> columns, bool isPrimaryKey, bool isUnique, bool isUniqueConstraint, bool isDisabled)
        {
            Table = table;
            Name = name;
            Columns = columns;
            IsPrimaryKey = isPrimaryKey;
            IsUnique = isUnique;
            IsUniqueConstraint = isUniqueConstraint;
            IsDisabled = isDisabled;
        }

        public TableInformation Table { get; }
        public string Name { get; }
        public IEnumerable<ColumnInformation> Columns { get; }
        public bool IsPrimaryKey { get; }
        public bool IsUnique { get; }
        public bool IsUniqueConstraint { get; }
        public bool IsDisabled { get; }
    }
}
