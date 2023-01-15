using System.Collections.Generic;
using System.Reflection;
using ADatabaseFixture;
using ADatabaseFixture.GalacticWasteManagement;
using GalacticWasteManagement;
using GalacticWasteManagement.Scripts;
using GalacticWasteManagement.Scripts.EmbeddedScripts;
using GalacticWasteManagement.Scripts.ScriptProviders;
using Xunit;

namespace DataDude.Tests.Core
{
    public class DatabaseFixture : DatabaseFixtureBase, IAsyncLifetime
    {
        public DatabaseFixture()
            : base(
                new SqlServerDatabaseAdapter(),
                GalacticWasteManagementMigrator.Create(new DefaultProjectSettings()))
        {
        }

        private class DefaultProjectSettings : ProjectSettings
        {
            public DefaultProjectSettings()
                : base(
                new DefaultMigrationVersioning(),
                new GalacticWasteManagement.SqlServer.MsSql150ScriptParser(),
                new List<IScriptProvider>
                {
                    new BuiltInScriptsScriptProvider(),
                    new EmbeddedScriptProvider(Assembly.GetAssembly(typeof(DatabaseFixture)), "Core.Scripts"),
                })
            {
            }
        }
    }
}
