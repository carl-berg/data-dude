using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataDude.Schema
{
    public class SchemaInformation : IEnumerable<TableInformation>
    {
        public SchemaInformation(IEnumerable<TableInformation> tables) => Tables = tables
            .ToDictionary(x => $"{x.Schema}.{x.Name}", x => x);

        protected IDictionary<string, TableInformation> Tables { get; private set; }

        public TableInformation? this[string tableName]
        {
            get
            {
                if (Tables.TryGetValue(tableName, out var schemaMatch))
                {
                    return schemaMatch;
                }

                var tableMatch = Find(tableName);

                if (tableMatch is [var single])
                {
                    return single;
                }
                else if (tableMatch.Where(x => x.Schema == "dbo").ToList() is [var singleDbo])
                {
                    return singleDbo;
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

        private IList<TableInformation> Find(string tableName)
        {
            if (tableName.Split('.') is { Length: 2 } fullTableName)
            {
                var schema = fullTableName[0].Trim('[', ']');
                var table = fullTableName[1].Trim('[', ']');
                return Tables.Values.Where(x => x.Schema == schema && x.Name == table).ToList();
            }

            return Tables.Values.Where(x => x.Name == tableName.Trim('[', ']')).ToList();
        }
    }
}
