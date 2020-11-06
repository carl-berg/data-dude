using System;
using ADatabaseFixture.GalacticWasteManagement;
using Respawn;
using Xunit;

namespace DataDude.Tests.Core
{
    [Collection("DatabaseIntegrationTest")]
    public abstract class DatabaseTest : IDisposable
    {
        public DatabaseTest(DatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        public static Checkpoint Checkpoint { get; } = new Checkpoint
        {
            TablesToIgnore = GalacticWasteManagementMigrator.VersioningTables,
        };

        public DatabaseFixture Fixture { get; }

        public void Dispose()
        {
            Checkpoint.Reset(Fixture.ConnectionString).GetAwaiter().GetResult();
        }
    }
}
