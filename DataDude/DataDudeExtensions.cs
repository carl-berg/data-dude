using DataDude.Instructions;

namespace DataDude
{
    public static class DataDudeExtensions
    {
        public static DataDude Execute(this DataDude dude, string sql, object? parameters = null)
        {
            dude.AddInstruction(new ExecuteInstruction(sql, parameters));
            return dude;
        }

        public static DataDude Insert(this DataDude dude, string table, object? data = null)
        {
            dude.AddInstruction(new InsertInstruction(table, data));
            return dude;
        }
    }
}
