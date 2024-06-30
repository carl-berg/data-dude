using System.Collections.Generic;
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
    public class AutoInsertFKTableTests
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
            a.AddForeignKey(new ForeignKeyInformation("FK_A_A", a, a, new[] { (a["Id"], a["Id"]) }));
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

            b.AddForeignKey(new ForeignKeyInformation("FK_B_A", b, a, new[] { (b["a_Id"], b["Id"]) }));

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

        [Fact]
        public async Task Handles_Previously_Inserted_Rows()
        {
            var schema = new TestSchema();
            var a = schema.AddTable("A");
            var b = schema.AddTable("B").AddFk(a);
            var c = schema.AddTable("C").AddFk(b);

            var context = new DataDudeContext(schema);
            await context.LoadSchema(null, null);

            // Simulate that dude.Go(..) from a previous execution has been executed and a row has already been inserted into table A
            InsertContext.Get(context).InsertedRows.Add(new InsertedRow(a, new Dictionary<string, object>(), null));

            context.Instructions.Add(new InsertInstruction("C"));
            var dependencyService = new DependencyService(DependencyTraversalStrategy.FollowAllForeignKeys);
            await new AddMissingInsertInstructionsPreProcessor(dependencyService).PreProcess(context);
            context.Instructions
                .OfType<InsertInstruction>()
                .Select(x => x.TableName)
                .ShouldBe(new[] { "[dbo].[B]", "C" });
        }

        [Fact]
        public void Can_Enable_AutoFks_Multiple_Times()
        {
            var dude = new Dude()
                .EnableAutomaticForeignKeys()
                .EnableAutomaticInsertOfForeignKeys();

            dude.Configure(context =>
            {
                context.InstructionDecorators.OfType<AddMissingInsertInstructionsPreProcessor>().ShouldHaveSingleItem();
            });

            dude.ConfigureInsert(insertContext =>
            {
                insertContext.InsertInterceptors.OfType<ForeignKeyInterceptor>().ShouldHaveSingleItem();
            });
        }

        [Fact]
        public void Can_Disable_AutoFks()
        {
            var dude = new Dude()
                .EnableAutomaticInsertOfForeignKeys()
                .DisableAutomaticForeignKeys();

            dude.Configure(context =>
            {
                context.InstructionDecorators.OfType<AddMissingInsertInstructionsPreProcessor>().ShouldBeEmpty();
            });

            dude.ConfigureInsert(insertContext =>
            {
                insertContext.InsertInterceptors.OfType<ForeignKeyInterceptor>().ShouldBeEmpty();
            });
        }
    }
}
