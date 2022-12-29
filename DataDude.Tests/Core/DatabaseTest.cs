using System;
using System.Threading.Tasks;
using ADatabaseFixture.GalacticWasteManagement;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace DataDude.Tests.Core
{
    [Collection("DatabaseIntegrationTest")]
    public abstract class DatabaseTest : IAsyncLifetime
    {
        public DatabaseTest(DatabaseFixture fixture) => Fixture = fixture;

        public DatabaseFixture Fixture { get; }

        private static Respawner Checkpoint { get; set; }

        public async Task InitializeAsync() => Checkpoint ??= await Respawner.CreateAsync(Fixture.ConnectionString);

        public Task DisposeAsync() => Checkpoint?.ResetAsync(Fixture.ConnectionString);
    }
}
