using Dapper;

namespace DataDude.Instructions.Execute
{
    public class ExecuteInstruction : IInstruction
    {
        public ExecuteInstruction(string sql, object? parameters = null)
        {
            Sql = sql;
            Parameters = new DynamicParameters(parameters);
        }

        public string Sql { get; }
        public DynamicParameters Parameters { get; }
    }
}
