using ADatabaseFixture;
using Microsoft.Data.SqlClient;
using Xunit;

namespace DataDude.Tests.Core
{
    public class DatabaseFixture() : DatabaseFixtureBase(
        new SqlServerDatabaseAdapter(ConnectionFactory),
        new FixtureMigrator()), IAsyncLifetime
    {
        public static SqlConnection ConnectionFactory(string connectionString) => new(connectionString);
    }
}
