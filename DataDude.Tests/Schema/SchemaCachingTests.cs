using System.Data.Common;
using System.Threading.Tasks;
using DataDude.Schema;
using FakeItEasy;
using Shouldly;
using Xunit;

namespace DataDude.Tests.Schema
{
    public class SchemaCachingTests
    {
        [Fact]
        public async Task Schema_Can_Be_Loaded()
        {
            var loader = A.Fake<ISchemaLoader>();
            var expectedSchema = new SchemaInformation([]);
            A.CallTo(() => loader.Load(A<DbConnection>.Ignored, A<DbTransaction>.Ignored)).Returns(expectedSchema);
            var dude = new Dude(loader, context => context.InstructionDecorators.Clear());

            await dude.Go(null, null);

            dude.Configure(context => context.Schema.ShouldBe(expectedSchema));
        }

        [Fact]
        public async Task Schema_Can_Be_Cached()
        {
            var schemaLoader = A.Fake<ISchemaLoader>();

            var dude = new Dude(schemaLoader, context => context.InstructionDecorators.Clear());
            await dude.Go(null, null);
            await dude.Go(null, null);

            A.CallTo(() => schemaLoader.Load(A<DbConnection>.Ignored, A<DbTransaction>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Schema_Cache_Can_Be_Turned_Off()
        {
            var schemaLoader = A.Fake<ISchemaLoader>();
            var dude = new Dude(schemaLoader, context => context.InstructionDecorators.Clear());

            await dude.Go(null, null);
            await dude.Go(null, null);
            dude.Configure(x => x.Set<SchemaInformation>("Schema", null));
            await dude.Go(null, null);

            A.CallTo(() => schemaLoader.Load(A<DbConnection>.Ignored, A<DbTransaction>.Ignored)).MustHaveHappenedTwiceExactly();
        }
    }
}
