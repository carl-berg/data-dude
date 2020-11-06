using Dapper;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataDude.Schema
{
    public class SchemaInformation : IEnumerable<TableInformation>
    {
        private IDictionary<string, TableInformation> _tables;
        public SchemaInformation(IEnumerable<TableInformation> tables) => _tables = tables
            .ToDictionary(x => $"{x.Schema}.{x.Name}", x => x);

        public TableInformation? this[string tableName]
        {
            get
            {
                if (_tables.TryGetValue(tableName, out var schemaMatch))
                {
                    return schemaMatch;
                }

                var tableMatch = _tables.Values.Where(x => x.Name == tableName).ToList();
                if (tableMatch.Count == 1)
                {
                    return tableMatch.Single();
                }
                else if (tableMatch.Count > 1)
                {
                    throw new SchemaInformationException($"Found multiple tables matching '{tableName}'");
                }

                return null;
            }
        }

        public IEnumerator<TableInformation> GetEnumerator() => _tables.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
