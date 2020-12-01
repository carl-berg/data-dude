namespace DataDude.Instructions.Execute
{
    public static class Extensions
    {
        public static DataDude Execute(this DataDude dude, string sql, object? parameters = null)
        {
            dude.Configure(x => x.Instructions.Add(new ExecuteInstruction(sql, parameters)));
            return dude;
        }
    }
}
