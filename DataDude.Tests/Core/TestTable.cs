using System.Linq;
using DataDude.Schema;

namespace DataDude.Tests.Core
{
    public class TestTable : TableInformation
    {
        public TestTable(string name)
            : this(name, "Id")
        {
            AddIndex(new IndexInformation(this, $"PK_{name}", new[] { this["Id"] ! }, true, false, false, false));
        }

        public TestTable(string name, params string[] columns)
            : base("dbo", name, table => columns.Select(c => new ColumnInformation(table, c, "int", true, false, false, null, 4, 0, 0)))
        {
        }

        public TestTable AddFk(params TableInformation[] to)
        {
            foreach (var referenceTable in to)
            {
                var fk = new ForeignKeyInformation(
                    $"FK_{Name}_{referenceTable.Name}",
                    this,
                    referenceTable,
                    new[] { (this["Id"], referenceTable["Id"]) });
                AddForeignKey(fk);
            }

            return this;
        }
    }
}
