using System.Linq;
using System.Threading.Tasks;
using DataDude.Instructions.Insert;
using DataDude.Instructions.Insert.AutomaticForeignKeys;
using DataDude.Schema;
using DataDude.Tests.Core;
using Shouldly;
using Xunit;

namespace DataDude.Tests.Inserts
{
    public class AutoInsertFKTableTestscs
    {
        [Fact]
        public async Task Chain()
        {
            var schema = new TestSchema();
            var a = schema.AddTable("A");
            var b = schema.AddTable("B").AddFk(a);
            var c = schema.AddTable("C").AddFk(b);

            var context = new DataDudeContext(schema);
            await context.LoadSchema(null, null);
            context.Instructions.Add(new InsertInstruction("C"));
            var dependencyService = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            await new AddMissingInsertInstructionsPreProcessor(dependencyService).PreProcess(context);
            context.Instructions
                .OfType<InsertInstruction>()
                .Select(x => x.TableName)
                .ShouldBe(new[] { "[dbo].[A]", "[dbo].[B]", "C" });
        }

        [Fact]
        public async Task Chain_Sparsely_Specified_Scenario_1()
        {
            var schema = new TestSchema();
            var a = schema.AddTable("A");
            var b = schema.AddTable("B").AddFk(a);
            var c = schema.AddTable("C").AddFk(b);
            var d = schema.AddTable("D").AddFk(c);

            var context = new DataDudeContext(schema);
            await context.LoadSchema(null, null);
            context.Instructions.Add(new InsertInstruction("B"));
            context.Instructions.Add(new InsertInstruction("D"));
            var dependencyService = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            await new AddMissingInsertInstructionsPreProcessor(dependencyService).PreProcess(context);
            context.Instructions
                .OfType<InsertInstruction>()
                .Select(x => x.TableName)
                .ShouldBe(new[] { "[dbo].[A]", "B", "[dbo].[C]", "D" });
        }

        [Fact]
        public async Task Chain_Sparsely_Specified_Scenario_2()
        {
            var schema = new TestSchema();
            var a = schema.AddTable("A");
            var b = schema.AddTable("B").AddFk(a);
            var c = schema.AddTable("C").AddFk(b);
            var d = schema.AddTable("D").AddFk(c);

            var context = new DataDudeContext(schema);
            await context.LoadSchema(null, null);
            context.Instructions.Add(new InsertInstruction("A"));
            context.Instructions.Add(new InsertInstruction("D"));
            var dependencyService = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            await new AddMissingInsertInstructionsPreProcessor(dependencyService).PreProcess(context);
            context.Instructions
                .OfType<InsertInstruction>()
                .Select(x => x.TableName)
                .ShouldBe(new[] { "A", "[dbo].[B]", "[dbo].[C]", "D" });
        }

        [Fact]
        public async Task Handles_Ignore_Recursive_Keys()
        {
            var schema = new TestSchema();
            var a = schema.AddTable("A");
            a.AddForeignKey(t => new ForeignKeyInformation("FK_A_A", t, t, new[] { (t["Id"], t["Id"]) }));
            var b = schema.AddTable("B").AddFk(a);

            var context = new DataDudeContext(schema);
            await context.LoadSchema(null, null);
            context.Instructions.Add(new InsertInstruction("B"));
            var dependencyService = new DependencyService(DependencyTraversalStrategy.SkipRecursiveForeignKeys);
            await new AddMissingInsertInstructionsPreProcessor(dependencyService).PreProcess(context);
            context.Instructions
                .OfType<InsertInstruction>()
                .Select(x => x.TableName)
                .ShouldBe(new[] { "[dbo].[A]", "B" });
        }

        [Fact]
        public async Task Handles_Ignore_Nullable_Keys()
        {
            var schema = new TestSchema();
            var a = schema.AddTable("A");
            var b = new TableInformation("dbo", "B", table => new[]
            {
                new ColumnInformation(table, "a_Id", "int", false, isNullable: true, false, null, 0, 0, 0),
            });
            b.AddForeignKey(t => new ForeignKeyInformation("FK_B_A", t, a, new[] { (t["a_Id"], a["Id"]) }));

            var context = new DataDudeContext(schema);
            await context.LoadSchema(null, null);
            context.Instructions.Add(new InsertInstruction("B"));
            var dependencyService = new DependencyService(DependencyTraversalStrategy.SkipNullableForeignKeys);
            await new AddMissingInsertInstructionsPreProcessor(dependencyService).PreProcess(context);
            context.Instructions
                .OfType<InsertInstruction>()
                .Select(x => x.TableName)
                .ShouldBe(new[] { "B" });
        }
    }
}
