using System.Collections.Generic;
using System.Linq;
using DataDude.Schema;
using DataDude.Tests.Core;
using Shouldly;
using Xunit;

namespace DataDude.Tests.Schema
{
    public class DependencyServiceTests
    {
        [Fact]
        public void Resolve_Chain()
        {
            var a = new TestTable("A");
            var b = new TestTable("B").AddFk(a);
            var c = new TestTable("C").AddFk(b);

            var service = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            service.GetOrderedDependenciesFor(c).ShouldBe(new[] { a, b }, true);
            service.GetOrderedDependenciesFor(b).ShouldBe(new[] { a });
            service.GetOrderedDependenciesFor(a).ShouldBeEmpty();
        }

        [Fact]
        public void Resolve_Fork()
        {
            var service = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            var a = new TestTable("A");
            var b = new TestTable("B");
            var c = new TestTable("C").AddFk(a, b);

            var dependencies = service.GetOrderedDependenciesFor(c);
            dependencies.ShouldBe(new[] { a, b }, true);
            AssertDepencyOrderFor(c, dependencies);
        }

        [Fact]
        public void Resolve_Forked_Branches()
        {
            var service = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            var a = new TestTable("A");
            var b = new TestTable("B");
            var c = new TestTable("C").AddFk(a, b);
            var d = new TestTable("D");
            var e = new TestTable("E").AddFk(c, d);

            var dependencies = service.GetOrderedDependenciesFor(e);
            dependencies.ShouldBe(new[] { a, b, c, d }, true);
            AssertDepencyOrderFor(e, dependencies);
        }

        [Fact]
        public void Resolve_Diamond()
        {
            var service = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            var a = new TestTable("A");
            var b = new TestTable("B").AddFk(a);
            var c = new TestTable("C").AddFk(a);
            var d = new TestTable("D").AddFk(b, c);

            var dependencies = service.GetOrderedDependenciesFor(d);
            dependencies.ShouldBe(new[] { a, b, c }, true);
            AssertDepencyOrderFor(d, dependencies);
        }

        [Fact]
        public void Resolve_Recursive_using_Follow_All_ForeignKeys()
        {
            var service = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            var a = new TestTable("A");
            a.AddForeignKey(new ForeignKeyInformation("FK", a, a, new[] { (a["Id"], a["Id"]) }));

            Should.Throw<DependencyTraversalFailedException>(() => service.GetOrderedDependenciesFor(a));
        }

        [Fact]
        public void Resolve_Recursive_using_Skip_Recursive_ForeignKeys()
        {
            var service = new DependencyService(DependencyTraversalStrategy.SkipRecursiveForeignKeys);
            var a = new TestTable("A");
            a.AddForeignKey(new ForeignKeyInformation("FK", a, a, new[] { (a["Id"], a["Id"]) }));

            var dependencies = Should.NotThrow(() => service.GetOrderedDependenciesFor(a));
            dependencies.ShouldBeEmpty();
        }

        [Fact]
        public void Resolve_Nullable_using_Follow_All_ForeignKeys()
        {
            var service = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);

            var a = new TestTable("A");
            var b = new TableInformation("dbo", "B", table => new[]
            {
                new ColumnInformation(table, "a_Id", "int", false, isNullable: true, false, null, 0, 0, 0),
            });
            b.AddForeignKey(new ForeignKeyInformation("FK", b, a, new[] { (b["a_Id"], a["Id"]) }));

            var dependencies = Should.NotThrow(() => service.GetOrderedDependenciesFor(b));
            dependencies.ShouldContain(a);
        }

        [Fact]
        public void Resolve_Nullable_using_Skip_Nullable_ForeignKeys()
        {
            var service = new DependencyService(DependencyTraversalStrategy.SkipNullableForeignKeys);

            var a = new TestTable("A");
            var b = new TableInformation("dbo", "B", table => new[]
            {
                new ColumnInformation(table, "a_Id", "int", false, isNullable: true, false, null, 0, 0, 0),
            });
            b.AddForeignKey(new ForeignKeyInformation("FK", b, a, new[] { (b["a_Id"], a["Id"]) }));

            var dependencies = Should.NotThrow(() => service.GetOrderedDependenciesFor(b));
            dependencies.ShouldBeEmpty();
        }

        private void AssertDepencyOrderFor(TableInformation table, IEnumerable<TableInformation> dependencies)
        {
            var previousDependencies = new HashSet<TableInformation>();
            foreach (var item in dependencies.Concat(new[] { table }))
            {
                item.ForeignKeys.Select(x => x.ReferencedTable).ShouldBeSubsetOf(
                    previousDependencies,
                    $"{item.Name} does not have its dependencies met");
                previousDependencies.Add(item);
            }
        }
    }
}
