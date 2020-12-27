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

        public TestTable AddTable(string name)
        {
            var table = new TestTable(name);
            Tables.Add(name, table);
            return table;
        }

        public Task<SchemaInformation> Load(IDbConnection connection, IDbTransaction transaction = null)
        {
            return Task.FromResult(this as SchemaInformation);
        }
    }
}
