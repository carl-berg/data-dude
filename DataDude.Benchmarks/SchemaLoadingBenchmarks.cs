using ADatabaseFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Dapper;
using DataDude.Schema;
using DataDude.SqlServer;

namespace Dude.Benchmarks
{
    [Config(typeof(Config))]
    [SimpleJob(RuntimeMoniker.Net60, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net70)]
    [MemoryDiagnoser]
    public class SchemaLoadingBenchmarks
    {
        private SqlServerSchemaLoader _schemaLoader = new SqlServerSchemaLoader();
        private SqlServerDatabaseAdapter _sqlDbAdapter;
        private System.Data.SqlClient.SqlConnection _system_data_connection;
        private Microsoft.Data.SqlClient.SqlConnection _microsoft_data_connection;

        [GlobalSetup]
        public async Task Setup()
        {
            _sqlDbAdapter = new SqlServerDatabaseAdapter($"DataDude_Schema_BenchMark_{DateTime.Now:yyyy-MM-dd_HH-mm}");
            _sqlDbAdapter.TryRemoveDatabase();

            var connectionString = _sqlDbAdapter.CreateDatabase();
            _system_data_connection = new System.Data.SqlClient.SqlConnection(connectionString);
            _microsoft_data_connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);

            using (var utilityConnection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                // Setup schema
                await utilityConnection.ExecuteAsync(NorthwindDatabase.Schema);
            }

            

            await _system_data_connection.OpenAsync();
            await _microsoft_data_connection.OpenAsync();
        }

        [Benchmark]
        public Task<SchemaInformation> ShemaLoader_System_Data_SqlConnection() => _schemaLoader.Load(_system_data_connection);

        [Benchmark]
        public Task<SchemaInformation> ShemaLoader_Microsoft_Data_SqlConnection() => _schemaLoader.Load(_microsoft_data_connection);

        [GlobalCleanup]
        public void TearDown()
        {
            _microsoft_data_connection.Dispose();
            _system_data_connection.Dispose();
            _sqlDbAdapter.TryRemoveDatabase();
        }
    }
}