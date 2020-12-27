using System.Data;
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
            var expectedSchema = new SchemaInformation(new TableInformation[0]);
            A.CallTo(() => loader.Load(A<IDbConnection>.Ignored, A<IDbTransaction>.Ignored)).Returns(expectedSchema);
            var dude = new Dude(loader);

            await dude.Go(null, null);

            dude.Configure(context => context.Schema.ShouldBe(expectedSchema));
        }

        [Fact]
        public async Task Schema_Can_Be_Cached()
        {
            var schemaLoader = A.Fake<ISchemaLoader>(o => o.ConfigureFake(loader => loader.CacheSchema = true));

            var dude = new Dude(schemaLoader);
            await dude.Go(null, null);
            await dude.Go(null, null);

            A.CallTo(() => schemaLoader.Load(A<IDbConnection>.Ignored, A<IDbTransaction>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Schema_Cache_Can_Be_Turned_Off()
        {
            var schemaLoader = A.Fake<ISchemaLoader>(o => o.ConfigureFake(loader => loader.CacheSchema = true));
            var dude = new Dude(schemaLoader);

            await dude.Go(null, null);
            await dude.Go(null, null);
            dude.Configure(x => x.SchemaLoader.CacheSchema = false);
            await dude.Go(null, null);

            A.CallTo(() => schemaLoader.Load(A<IDbConnection>.Ignored, A<IDbTransaction>.Ignored)).MustHaveHappenedTwiceExactly();
        }
    }
}
