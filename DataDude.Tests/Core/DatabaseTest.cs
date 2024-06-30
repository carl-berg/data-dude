using System.Threading.Tasks;
using Respawn;
using Xunit;

namespace DataDude.Tests.Core
{
    [Collection("DatabaseIntegrationTest")]
    public abstract class DatabaseTest(DatabaseFixture fixture) : IAsyncLifetime
    {
        public DatabaseFixture Fixture { get; } = fixture;

        private static Respawner Checkpoint { get; set; }

        public async Task InitializeAsync() => Checkpoint ??= await Respawner.CreateAsync(Fixture.ConnectionString);

        public Task DisposeAsync() => Checkpoint?.ResetAsync(Fixture.ConnectionString);
    }
}
