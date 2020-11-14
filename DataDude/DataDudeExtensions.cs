using DataDude.Instructions.Execute;
using DataDude.Instructions.Insert;
using DataDude.Instructions.Insert.Interception;

namespace DataDude
{
    public static class DataDudeExtensions
    {
        public static DataDude Execute(this DataDude dude, string sql, object? parameters = null)
        {
            dude.Configure(x => x.Instructions.Add(new ExecuteInstruction(sql, parameters)));
            return dude;
        }

        public static DataDude Insert(this DataDude dude, string table, object? data = null)
        {
            dude.Configure(x => x.Instructions.Add(new InsertInstruction(table, data)));
            return dude;
        }

        public static DataDude EnableAutomaticForeignKeys(this DataDude dude)
        {
            dude.Configure(x => x.InsertInterceptors.Add(new ForeignKeyInterceptor()));
            return dude;
        }
    }
}
