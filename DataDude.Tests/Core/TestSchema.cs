using DataDude.Schema;

namespace DataDude.Tests.Core
{
    public class TestSchema : SchemaInformation
    {
        public TestSchema()
            : base(new TableInformation[0])
        {
        }

        public TestTable AddTable(string name)
        {
            var table = new TestTable(name);
            Tables.Add(name, table);
            return table;
        }
    }
}
