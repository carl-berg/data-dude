using System.Threading.Tasks;
using Dapper;
using DataDude.Instructions.Insert;
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

        [Fact]
        public async Task Test_Can_Insert_Instruction()
        {
            using var connection = Fixture.CreateNewConnection();

            await new DataDude()
                .Insert("Office", new { Name = "test" })
                .Go(connection);

            var officeName = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            officeName.ShouldBe("test");
        }

        [Fact]
        public async Task Test_Can_Insert_Instruction_With_Default()
        {
            using var connection = Fixture.CreateNewConnection();

            await new DataDude()
                .Insert("Office")
                .Go(connection);

            var officeName = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            officeName.ShouldBeEmpty();
        }

        [Fact]
        public async Task Test_Can_Insert_With_Explicit_Foreign_Keys()
        {
            using var connection = Fixture.CreateNewConnection();

            await new DataDude()
                .Insert("Office", new { Id = 1 })
                .Insert("Employee", new { Id = 1 })
                .Insert("OfficeOccupant", new { OfficeId = 1, EmployeeId = 1 })
                .Go(connection);

            var occupants = await connection.QueryAsync<dynamic>("SELECT * FROM Buildings.OfficeOccupant");
            occupants.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Test_Can_Insert_With_Automatic_Foreign_Keys()
        {
            using var connection = Fixture.CreateNewConnection();

            await new DataDude()
                .EnableAutomaticForeignKeys()
                .Insert("Office")
                .Insert("Employee")
                .Insert("OfficeOccupant")
                .Go(connection);

            var occupants = await connection.QueryAsync<dynamic>("SELECT * FROM Buildings.OfficeOccupant");
            occupants.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Test_Can_Insert_With_Automatic_Foreign_Keys_PK_Is_Also_FK()
        {
            using var connection = Fixture.CreateNewConnection();

            await new DataDude()
                .EnableAutomaticForeignKeys()
                .Insert("Office")
                .Insert("OfficeExtension")
                .Go(connection);

            var extensions = await connection.QueryAsync<dynamic>("SELECT * FROM Buildings.OfficeExtension");
            extensions.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Test_Can_Insert_With_Raw_SQL()
        {
            using var connection = Fixture.CreateNewConnection();

            await new DataDude()
                .EnableAutomaticForeignKeys()
                .Insert("Office", new { Name = new RawSql("CONCAT('A', 'B', 'C')") })
                .Go(connection);

            var name = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            name.ShouldBe("ABC");
        }
    }
}
