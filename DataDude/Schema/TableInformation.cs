using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataDude.Schema
{
    public class TableInformation : IEnumerable<ColumnInformation>
    {
        private readonly IDictionary<string, ColumnInformation> _columns;
        private readonly IList<ForeignKeyInformation> _foreignKeys;
        public TableInformation(string schema, string name, IEnumerable<ColumnInformation> columns)
        {
            Schema = schema;
            Name = name;
            _columns = columns.ToDictionary(x => x.Name, x => x);
            _foreignKeys = new List<ForeignKeyInformation>();
        }

        public string Schema { get; }
        public string Name { get; }
        public IEnumerable<ForeignKeyInformation> ForeignKeys => _foreignKeys;
        public ColumnInformation? this[string name] => _columns.TryGetValue(name, out var column) ? column : null;
        public IEnumerator<ColumnInformation> GetEnumerator() => _columns.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void AddForeignKey(ForeignKeyInformation fk) => _foreignKeys.Add(fk);
    }
}
