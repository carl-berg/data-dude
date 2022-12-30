using BenchmarkDotNet.Configs;

namespace Dude.Benchmarks
{
    internal class Config : ManualConfig
    {
        public Config()
        {
            // A-Databasefixture is debug-compiled preventing normal validation
            WithOptions(ConfigOptions.DisableOptimizationsValidator);
        }
    }
}