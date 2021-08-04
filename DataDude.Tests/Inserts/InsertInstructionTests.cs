using System.Collections.Generic;
using System.Dynamic;
using DataDude.Instructions.Insert;
using Shouldly;
using Xunit;

namespace DataDude.Tests.Inserts
{
    public class InsertInstructionTests
    {
        [Fact]
        public void Can_Parse_Insert_Data_From_Anonymous_Object()
        {
            var data = new { A = "A string", B = 5 };
            var instruction = new InsertInstruction("Table", data);

            instruction.ColumnValues.ShouldContainKey("A");
            instruction.ColumnValues.ShouldContainKey("B");
        }

        [Fact]
        public void Can_Parse_Insert_Data_From_Class_Object()
        {
            var data = new ClassBasedInstruction("A string", 5);
            var instruction = new InsertInstruction("Table", data);

            instruction.ColumnValues.ShouldContainKey("A");
            instruction.ColumnValues.ShouldContainKey("B");
        }

        [Fact]
        public void Can_Parse_Insert_Data_From_Dynamic_Object()
        {
            dynamic data = new ExpandoObject();
            data.A = "A string";
            data.B = 5;

            var instruction = new InsertInstruction("A", data);

            instruction.ColumnValues.ShouldContainKey("A");
            instruction.ColumnValues.ShouldContainKey("B");
        }

        [Fact]
        public void Can_Parse_Insert_Data_From_Dictionary_Object()
        {
            var data = new Dictionary<string, object>
            {
                ["A"] = "A string",
                ["B"] = 5,
            };

            var instruction = new InsertInstruction("A", data);

            instruction.ColumnValues.ShouldContainKey("A");
            instruction.ColumnValues.ShouldContainKey("B");
        }

        public class ClassBasedInstruction
        {
            public ClassBasedInstruction(string a, int b) => (A, B) = (a, b);
            public string A { get; }
            public int B { get; }
        }
    }
}
