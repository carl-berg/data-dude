using System.Threading.Tasks;
using DataDude.SqlServer;
using DataDude.Tests.Core;
using Shouldly;
using Xunit;

namespace DataDude.Tests.Schema
{
    public class SchemaIntegrationTests : DatabaseTest
    {
        public SchemaIntegrationTests(DatabaseFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task Schema_Loading()
        {
            using var connection = Fixture.CreateNewConnection();
            var schema = await new SqlServerSchemaLoader().Load(connection);

            schema["Office"].ShouldNotBeNull();
            schema["Buildings.Office"].ShouldNotBeNull();
            schema["Employee"].ShouldSatisfyAllConditions(
                table => table["Id"].DataType.ShouldBe("int"),
                table => table["Id"].DefaultValue.ShouldBeNull(),
                table => table["Id"].IsIdentity.ShouldBeTrue(),
                table => table["Id"].IsNullable.ShouldBeFalse(),
                table => table["FullName"].IsComputed.ShouldBeTrue(),
                table => table["Active"].DefaultValue.ShouldBe("((1))"),
                table => table["CreatedAt"].DefaultValue.ShouldBe("(getdate())"),
                table => table.ForeignKeys.ShouldBeEmpty(),
                table => table.Triggers.ShouldContain(x => x.Name == "EmployeeUpdatedAt"));

            schema["OfficeOccupant"].ShouldHaveForeignKey(
                constraint: "FK_OfficeOccupant_Employee",
                referencedTable: "Employee",
                columns: ("EmployeeId", "Id"));

            schema["OfficeOccupant"].ShouldHaveForeignKey(
                constraint: "FK_OfficeOccupant_Office",
                referencedTable: "Office",
                columns: ("OfficeId", "Id"));

            schema["OfficeOccupancy"].ShouldHaveForeignKey(
                constraint: "FK_OfficeOccupancy_OfficeOccupant",
                referencedTable: "OfficeOccupant",
                ("OfficeId", "OfficeId"),
                ("EmployeeId", "EmployeeId"));

            schema["Test_Generated_PK_Scenario_1"].Indexes.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
                index => index.Name.ShouldBe("PK_Test_PK_Scenarios_1"),
                index => index.IsPrimaryKey.ShouldBeTrue());
        }
    }
}
