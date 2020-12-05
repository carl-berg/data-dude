using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataDude.Schema
{
    public class SchemaInformation : IEnumerable<TableInformation>
    {
        public SchemaInformation(IEnumerable<TableInformation> tables) => Tables = tables
            .ToDictionary(x => $"{x.FullName}", x => x);

        protected IDictionary<string, TableInformation> Tables { get; private set; }

        public TableInformation? this[string tableName]
        {
            get
            {
                if (Tables.TryGetValue(tableName, out var schemaMatch))
                {
                    return schemaMatch;
                }

                var tableMatch = Tables.Values.Where(x => x.Name == tableName).ToList();
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

        public IEnumerator<TableInformation> GetEnumerator() => Tables.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
