using System.Data;
using System.Threading.Tasks;
using DataDude.Schema;

namespace DataDude.Tests.Core
{
    public class TestSchema : SchemaInformation, ISchemaLoader
    {
        public TestSchema()
            : base(new TableInformation[0])
        {
        }

        public bool CacheSchema { get; set; }

        public TestTable AddTable(string name) => AddTable(new TestTable(name));

        public TestTable AddTable(TestTable table)
        {
            Tables.Add(table.Name, table);
            return table;
        }

        public Task<SchemaInformation> Load(IDbConnection connection, IDbTransaction transaction = null)
        {
            return Task.FromResult(this as SchemaInformation);
        }
    }
}
