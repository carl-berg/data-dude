using ADatabaseMigrator.Hashing;
using ADatabaseMigrator.Journaling;
using ADatabaseMigrator.ScriptLoading.EmbeddedResources;
using ADatabaseMigrator;
using ADatabaseMigrator.ScriptLoading.EmbeddedResources.Versioning;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Threading;
using ADatabaseMigrator.SqlServer;

namespace DataDude.Tests.Core
{
    public class FixtureMigrator : ADatabaseFixture.IMigrator
    {
        public const string VersioningTable = MigrationScriptJournalManager.JournalTableName;

        public async Task MigrateUp(string connectionString, CancellationToken? cancellationToken)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            await new Migrator(
                    scriptLoader: new EmbeddedResourceScriptLoader(new MD5ScriptHasher(), config => config
                        .UsingAssemblyFromType<FixtureMigrator>()
                            .AddNamespaces<VersionFromPathVersionLoader>(MigrationScriptRunType.RunOnce, "Core.Scripts.Migrations")),
                    journalManager: new MigrationScriptJournalManager(connection),
                    scriptRunner: new SqlServerMigrationScriptRunner(connection))
                .Migrate(cancellationToken);
        }
    }
}
