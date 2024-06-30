using System.Threading.Tasks;
using Dapper;
using DataDude.Tests.Core;
using Shouldly;
using Xunit;

namespace DataDude.Tests
{
    public class ExecutionTests(DatabaseFixture fixture) : DatabaseTest(fixture)
    {
        [Fact]
        public async Task Can_Execute_Instruction()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .Execute("INSERT INTO Buildings.Office(Name) VALUES(@Name)", new { Name = "test" })
                .Go(connection);

            var officeName = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            officeName.ShouldBe("test");
        }
    }
}
