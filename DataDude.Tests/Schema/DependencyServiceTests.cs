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

            var service = new DependencyService();
            service.GetOrderedDependenciesFor(c).ShouldBe(new[] { a, b }, true);
            service.GetOrderedDependenciesFor(b).ShouldBe(new[] { a });
            service.GetOrderedDependenciesFor(a).ShouldBeEmpty();
        }

        [Fact]
        public void Resolve_Fork()
        {
            var service = new DependencyService();
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
            var service = new DependencyService();
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
            var service = new DependencyService();
            var a = new TestTable("A");
            var b = new TestTable("B").AddFk(a);
            var c = new TestTable("C").AddFk(a);
            var d = new TestTable("D").AddFk(b, c);

            var dependencies = service.GetOrderedDependenciesFor(d);
            dependencies.ShouldBe(new[] { a, b, c }, true);
            AssertDepencyOrderFor(d, dependencies);
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
