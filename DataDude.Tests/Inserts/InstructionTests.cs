using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Instructions.Insert;
using DataDude.Instructions.Insert.Insertion;
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
        public async Task Can_Insert_Instruction()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .Insert("Office", new { Name = "test" })
                .Go(connection);

            var officeName = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            officeName.ShouldBe("test");
        }

        [Fact]
        public async Task Can_Insert_With_Brackets()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .Insert("[Office]")
                .Go(connection);

            var numberOfOffices = await connection.QuerySingleAsync<int>("SELECT COUNT(1) FROM Buildings.Office");
            numberOfOffices.ShouldBe(1);
        }

        [Fact]
        public async Task Can_Insert_Instruction_With_Default()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .Insert("Office")
                .Go(connection);

            var officeName = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            officeName.ShouldBeEmpty();
        }

        [Fact]
        public async Task Can_Insert_With_Explicit_Foreign_Keys()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .Insert("Office", new { Id = 1 })
                .Insert("Employee", new { Id = 1 })
                .Insert("OfficeOccupant", new { OfficeId = 1, EmployeeId = 1 })
                .Go(connection);

            var occupants = await connection.QueryAsync<dynamic>("SELECT * FROM Buildings.OfficeOccupant");
            occupants.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Can_Insert_With_Automatic_Foreign_Keys()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys()
                .Insert("Office")
                .Insert("Employee")
                .Insert("OfficeOccupant")
                .Go(connection);

            var occupants = await connection.QueryAsync<dynamic>("SELECT * FROM Buildings.OfficeOccupant");
            occupants.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Can_Insert_With_Automatic_Foreign_Keys_Does_Not_Interfere_With_Specified_Keys()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys()
                .Insert("Office")
                .Insert("Employee", new { Id = 1 }, new { Id = 2 })
                .Insert("OfficeOccupant", new { EmployeeId = 1 }, new { EmployeeId = 2 })
                .Go(connection);

            var numberOfOccupants = await connection.QuerySingleAsync<int>("SELECT COUNT(1) FROM Buildings.OfficeOccupant");
            numberOfOccupants.ShouldBe(2);
        }

        [Fact]
        public async Task Can_Insert_With_Automatic_Foreign_Keys_Do_Not_Use_Value_Provided_Values_For_Foreign_Keys()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys()
                .Insert("Office")
                .Insert("Employee")
                .Insert("OfficeOccupant")
                .Insert("OfficeOccupancy")
                .Go(connection);

            var result = await connection.QueryAsync<dynamic>("SELECT * FROM People.OfficeOccupancy");
            result.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Can_Insert_With_Automatic_Foreign_Keys_PK_Is_Also_FK()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys()
                .Insert("Office")
                .Insert("OfficeExtension")
                .Go(connection);

            var extensions = await connection.QueryAsync<dynamic>("SELECT * FROM Buildings.OfficeExtension");
            extensions.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Can_Insert_With_Automatic_Foreign_Keys_And_Add_Missing_Insert_Instructions()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys(x => x.AddMissingForeignKeys = true)
                .Insert("OfficeOccupant")
                .Go(connection);

            var occupants = await connection.QueryAsync<dynamic>("SELECT * FROM Buildings.OfficeOccupant");
            occupants.ShouldHaveSingleItem();
        }

        [Fact]
        public async Task Can_Insert_With_Generated_PK_Scenario_1()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys()
                .Insert("Test_Generated_PK_Scenario_1")
                .Insert("Test_Generated_PK_Scenario_1")
                .Go(connection);

            var scenarioRows = await connection.QueryAsync<object>("SELECT * FROM Test_Generated_PK_Scenario_1");
            scenarioRows.Count().ShouldBe(2);
        }

        [Fact]
        public async Task Can_Insert_With_Generated_PK_Scenario_2()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys()
                .Insert("Test_Generated_PK_Scenario_2")
                .Insert("Test_Generated_PK_Scenario_2")
                .Go(connection);

            var scenarioRows = await connection.QueryAsync<object>("SELECT * FROM Test_Generated_PK_Scenario_2");
            scenarioRows.Count().ShouldBe(2);
        }

        [Fact]
        public async Task Can_Insert_With_Raw_SQL()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys()
                .Insert("Office", new { Name = new RawSql("CONCAT('A', 'B', 'C')") })
                .Go(connection);

            var name = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            name.ShouldBe("ABC");
        }

        [Fact]
        public async Task Can_Specify_Custom_Defaults()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .ConfigureCustomColumnValue((column, value) =>
                {
                    if (column.Name == "Name")
                    {
                        value.Set(new ColumnValue("Custom default"));
                    }
                })
                .Insert("Office")
                .Go(connection);

            var name = await connection.QuerySingleAsync<string>("SELECT Name FROM Buildings.Office");
            name.ShouldBe("Custom default");
        }

        [Fact]
        public async Task Can_Insert_Multiple_Instructions()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .Insert("Office", new { Name = "test" }, new { Name = "test" })
                .Go(connection);

            var rows = await connection.QueryFirstAsync<int>("SELECT Count(1) FROM Buildings.Office");
            rows.ShouldBe(2);
        }

        [Fact]
        public async Task Cannot_Insert_Into_Table_Without_PK()
        {
            using var connection = Fixture.CreateNewConnection();

            var exception = await Should.ThrowAsync<InsertRowHandlerMissing>(new Dude()
                .Insert("Test_PK_Less_Scenario")
                .Go(connection));
        }

        [Fact]
        public async Task Can_Insert_Into_Table_Without_PK_With_OutputInsertRowHandler_Configured()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .ConfigureInsert(x => x.InsertRowHandlers.Add(new OutputInsertRowHandler()))
                .Insert("Test_PK_Less_Scenario")
                .Go(connection);

            var rows = await connection.QueryFirstAsync<int>("SELECT Count(1) FROM Test_PK_Less_Scenario");
            rows.ShouldBe(1);
        }

        [Fact]
        public async Task Can_Insert_Sequential_Default_Using_Generated_Value_Instead_Of_Default()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .Insert("Test_PK_Sequential_Uuid")
                .Go(connection);

            var rows = await connection.QueryFirstAsync<int>("SELECT Count(1) FROM Test_PK_Sequential_Uuid");
            rows.ShouldBe(1);
        }

        [Fact]
        public async Task Can_Honor_Specified_Null_Values_When_Inserting_Nullable_FK()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .EnableAutomaticForeignKeys()
                .Insert("Office", new { Id = 1 })
                .Insert("Test_Nullable_FK", new { Id = 1 })
                .Insert("Test_Nullable_FK", new { Id = 2, OfficeId = (int?)null })
                .Go(connection);

            var firstOfficeId = await connection.QuerySingleAsync<int?>("SELECT OfficeId FROM Test_Nullable_FK WHERE Id = 1");
            var secondOfficeId = await connection.QuerySingleAsync<int?>("SELECT OfficeId FROM Test_Nullable_FK WHERE Id = 2");
            firstOfficeId.ShouldBe(1);
            secondOfficeId.ShouldBeNull();
        }

        [Fact]
        public async Task Can_Handle_Nullable_Text_Data_Type()
        {
            using var connection = Fixture.CreateNewConnection();

            await new Dude()
                .Insert("Test_Nullable_Text_Data_Type")
                .Go(connection);

            var insertedItems = await connection.QueryAsync<(int, string)>("SELECT Id, Text FROM Test_Nullable_Text_Data_Type");
            insertedItems.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Instructions_Are_Cleared_After_Go()
        {
            using var connection = Fixture.CreateNewConnection();

            var dude = new Dude();
            await dude
                .Insert("Office", new { Name = "test" })
                .Go(connection);

            await dude
                .Insert("Employee", new { Name = "test" })
                .Go(connection);

            var insertedOffices = await connection.QuerySingleAsync<int>("SELECT COUNT(1) FROM Buildings.Office");
            var insertedEmployees = await connection.QuerySingleAsync<int>("SELECT COUNT(1) FROM People.Employee");
            insertedOffices.ShouldBe(1);
            insertedEmployees.ShouldBe(1);
        }
    }
}
