using System.Linq;
using System.Threading.Tasks;
using DataDude.Instructions.Insert;
using DataDude.Instructions.Insert.AutomaticForeignKeys;
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

            var context = new DataDudeContext() { Schema = schema };

            context.Instructions.Add(new InsertInstruction("C"));
            await new AddMissingInsertInstructionsPreProcessor().PreProcess(context);
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

            var context = new DataDudeContext() { Schema = schema };

            context.Instructions.Add(new InsertInstruction("B"));
            context.Instructions.Add(new InsertInstruction("D"));
            await new AddMissingInsertInstructionsPreProcessor().PreProcess(context);
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

            var context = new DataDudeContext() { Schema = schema };

            context.Instructions.Add(new InsertInstruction("A"));
            context.Instructions.Add(new InsertInstruction("D"));
            await new AddMissingInsertInstructionsPreProcessor().PreProcess(context);
            context.Instructions
                .OfType<InsertInstruction>()
                .Select(x => x.TableName)
                .ShouldBe(new[] { "A", "[dbo].[B]", "[dbo].[C]", "D" });
        }
    }
}
