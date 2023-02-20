using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using DataDude.Schema;

namespace DataDude.Tests.Core
{
    public class TestSchema : SchemaInformation, ISchemaLoader
    {
        public TestSchema()
            : base(System.Array.Empty<TableInformation>())
        {
        }

        public bool CacheSchema { get; set; }

        public TestTable AddTable(string name) => AddTable(new TestTable(name));

        public TestTable AddTable(TestTable table)
        {
            Tables.Add(table.Name, table);
            return table;
        }

        public ValueTask<SchemaInformation> Load(DbConnection connection, DbTransaction transaction = null)
        {
            return new ValueTask<SchemaInformation>(this);
        }
    }
}
