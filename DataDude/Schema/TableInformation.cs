using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataDude.Schema
{
    [DebuggerDisplay("{FullName}")]
    public class TableInformation : IEnumerable<ColumnInformation>
    {
        private readonly IDictionary<string, ColumnInformation> _columns;
        private readonly IList<ForeignKeyInformation> _foreignKeys;
        private readonly IList<TriggerInformation> _triggers;
        private readonly IList<IndexInformation> _indexes;
        public TableInformation(string schema, string name, Func<TableInformation, IEnumerable<ColumnInformation>> getColumns)
        {
            Schema = schema;
            Name = name;
            _columns = getColumns(this).ToDictionary(x => x.Name, x => x);
            _foreignKeys = new List<ForeignKeyInformation>();
            _triggers = new List<TriggerInformation>();
            _indexes = new List<IndexInformation>();
        }

        public string Schema { get; }
        public string Name { get; }
        public string FullName => $"[{Schema}].[{Name}]";
        public IEnumerable<ForeignKeyInformation> ForeignKeys => _foreignKeys;
        public IEnumerable<TriggerInformation> Triggers => _triggers;
        public IEnumerable<IndexInformation> Indexes => _indexes;
        public ColumnInformation? this[string name] => _columns.TryGetValue(name, out var column) ? column : null;
        public IEnumerator<ColumnInformation> GetEnumerator() => _columns.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void AddForeignKey(Func<TableInformation, ForeignKeyInformation> getFk) => _foreignKeys.Add(getFk(this));
        public void AddTrigger(TriggerInformation trigger) => _triggers.Add(trigger);
        public void AddIndex(IndexInformation index) => _indexes.Add(index);

        public override string ToString() => Name;
    }
}
