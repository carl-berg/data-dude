using DataDude.Instructions.Execute;

namespace DataDude
{
    public static class ExecuteExtensions
    {
        public static Dude Execute(this Dude dude, string sql, object? parameters = null)
        {
            dude.Configure(x => x.Instructions.Add(new ExecuteInstruction(sql, parameters)));
            return dude;
        }
    }
}
