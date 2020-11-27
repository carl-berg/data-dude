using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataDude.Schema
{
    public class TableInformation : IEnumerable<ColumnInformation>
    {
        private readonly IDictionary<string, ColumnInformation> _columns;
        private readonly IList<ForeignKeyInformation> _foreignKeys;
        private readonly IList<TriggerInformation> _triggers;
        public TableInformation(string schema, string name, Func<TableInformation, IEnumerable<ColumnInformation>> getColumns)
        {
            Schema = schema;
            Name = name;
            _columns = getColumns(this).ToDictionary(x => x.Name, x => x);
            _foreignKeys = new List<ForeignKeyInformation>();
            _triggers = new List<TriggerInformation>();
        }

        public string Schema { get; }
        public string Name { get; }
        public string FullName => $"{Schema}.{Name}";
        public IEnumerable<ForeignKeyInformation> ForeignKeys => _foreignKeys;
        public IEnumerable<TriggerInformation> Triggers => _triggers;
        public ColumnInformation? this[string name] => _columns.TryGetValue(name, out var column) ? column : null;
        public IEnumerator<ColumnInformation> GetEnumerator() => _columns.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void AddForeignKey(ForeignKeyInformation fk) => _foreignKeys.Add(fk);
        public void AddTrigger(TriggerInformation trigger) => _triggers.Add(trigger);
    }
}
