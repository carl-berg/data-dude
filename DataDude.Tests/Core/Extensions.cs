using System.Linq;
using DataDude.Schema;
using Shouldly;

namespace DataDude.Tests.Core
{
    public static class Extensions
    {
        public static void ShouldHaveForeignKey(this TableInformation table, string constraint, string referencedTable, params (string column, string referencedColumn)[] columns)
        {
            var actual = table.ForeignKeys.Where(x => x.Name == constraint).ShouldHaveSingleItem();

            actual.ShouldSatisfyAllConditions(
                fk => fk.Name.ShouldBe(constraint),
                fk => fk.ReferencedTable.Name.ShouldBe(referencedTable));

            foreach (var (column, referencedColumn) in columns)
            {
                actual.Columns.ShouldContain(x => x.Column.Name == column && x.ReferencedColumn.Name == referencedColumn);
            }
        }
    }
}
