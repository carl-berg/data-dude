using ADatabaseFixture.GalacticWasteManagement;
using Respawn;
using System;
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

        public DatabaseFixture Fixture { get; }

        public static Checkpoint Checkpoint { get; } = new Checkpoint
        {
            TablesToIgnore = GalacticWasteManagementMigrator.VersioningTables,
        };

        public void Dispose()
        {
            Checkpoint.Reset(Fixture.ConnectionString).GetAwaiter().GetResult();
        }
    }
}
