using DataDude.Schema;
using DataDude.Tests.Core;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace DataDude.Tests
{
    public class SchemaTests : DatabaseTest
    {
        public SchemaTests(DatabaseFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Test_Schema_Loading()
        {
            using var connection = Fixture.CreateNewConnection();
            
            var schema = await SchemaInformation.Load(connection);

            schema["Office"].ShouldNotBeNull();
            schema["Buildings.Office"].ShouldNotBeNull();
            schema["Employee"].ShouldSatisfyAllConditions(
                table => table["Id"].DataType.ShouldBe("int"),
                table => table["Id"].ForeignKeys.ShouldBeEmpty(),
                table => table["Id"].HasDefaultValue.ShouldBeFalse(),
                table => table["Id"].IsIdentity.ShouldBeTrue(),
                table => table["Id"].IsNullable.ShouldBeFalse(),
                table => table["FullName"].IsComputed.ShouldBeTrue(),
                table => table["Active"].HasDefaultValue.ShouldBeTrue(),
                table => table["OfficeId"].ForeignKeys.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
                    fk => fk.Name.ShouldBe("FK_Employee_Office"),
                    fk => fk.Table.Name.ShouldBe("Office"),
                    fk => fk.Column.Name.ShouldBe("Id")));
        }
    }
}
