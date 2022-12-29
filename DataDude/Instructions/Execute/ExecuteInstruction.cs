namespace DataDude.Instructions.Execute
{
    public class ExecuteInstruction : IInstruction
    {
        public ExecuteInstruction(string sql, object? parameters = null)
        {
            Sql = sql;
            Parameters = new ColumnValues(parameters);
        }

        public string Sql { get; }
        public ColumnValues Parameters { get; }
    }
}
