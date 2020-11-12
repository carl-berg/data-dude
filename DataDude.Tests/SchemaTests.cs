using System.Threading.Tasks;
using DataDude.SqlServer;
using DataDude.Tests.Core;
using Shouldly;
using Xunit;

namespace DataDude.Tests
{
    public class SchemaTests : DatabaseTest
    {
        public SchemaTests(DatabaseFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task Test_Schema_Loading()
        {
            using var connection = Fixture.CreateNewConnection();

            var schema = await new SqlServerSchemaLoader().Load(connection);

            schema["Office"].ShouldNotBeNull();
            schema["Buildings.Office"].ShouldNotBeNull();
            schema["Employee"].ShouldSatisfyAllConditions(
                table => table["Id"].DataType.ShouldBe("int"),
                table => table["Id"].HasDefaultValue.ShouldBeFalse(),
                table => table["Id"].IsIdentity.ShouldBeTrue(),
                table => table["Id"].IsNullable.ShouldBeFalse(),
                table => table["FullName"].IsComputed.ShouldBeTrue(),
                table => table["Active"].HasDefaultValue.ShouldBeTrue(),
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
        }
    }
}
