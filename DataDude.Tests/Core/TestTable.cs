using DataDude.Schema;

namespace DataDude.Tests.Core
{
    public class TestTable : TableInformation
    {
        public TestTable(string name)
            : base("dbo", name, table => new[]
        {
                new ColumnInformation(table, "Id", "int", true, true, false, false, null, 4, 0, 0),
        })
        {
        }

        public TestTable AddFk(params TableInformation[] to)
        {
            foreach (var referenceTable in to)
            {
                var fk = new ForeignKeyInformation(
                    $"FK_{Name}_{referenceTable.Name}",
                    referenceTable,
                    new[] { (this["Id"], referenceTable["Id"]) });
                AddForeignKey(fk);
            }

            return this;
        }
    }
}
