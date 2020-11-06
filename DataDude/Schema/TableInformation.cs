using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataDude.Schema
{
    public class TableInformation : IEnumerable<ColumnInformation>
    {
        private IDictionary<string, ColumnInformation> _columns;
        public TableInformation(string schema, string name, IEnumerable<ColumnInformation> columns)
        {
            Schema = schema;
            Name = name;
            _columns = columns.ToDictionary(x => x.Name, x => x);
        }

        public string Schema { get; }
        public string Name { get; }

        public ColumnInformation? this[string name] => _columns.TryGetValue(name, out var column) ? column : null;
        public IEnumerator<ColumnInformation> GetEnumerator() => _columns.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
