using Dapper;

namespace DataDude.Instructions
{
    public class ExecuteInstruction : IDataDudeInstruction
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
