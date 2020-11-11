using System.Threading.Tasks;
using Dapper;
using DataDude.SqlServer;
using DataDude.Tests.Core;
using Shouldly;
using Xunit;

namespace DataDude.Tests
{
    public class InstructionTests : DatabaseTest
    {
        public InstructionTests(DatabaseFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task Test_Can_Execute_Instruction()
        {
            using var connection = Fixture.CreateNewConnection();

            await new DataDude()
                .Execute("INSERT INTO Buildings.Office(Name) VALUES(@Name)", new { Name = "test" })
                .Go(connection);

            var officeName = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            officeName.ShouldBe("test");
        }
    }
}
