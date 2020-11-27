using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Instructions.Execute;
using DataDude.Instructions.Insert;
using DataDude.Instructions.Insert.Interception;
using DataDude.Schema;

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
            dude.ConfigureInsert(x => x.InsertInterceptors.Add(new ForeignKeyInterceptor()));
            return dude;
        }

        public static DataDude ConfigureInsert(this DataDude dude, Action<InsertContext> configure)
        {
            dude.Configure(x =>
            {
                if (InsertContext.Get(x) is { } context)
                {
                    configure(context);
                }
            });
            return dude;
        }
    }
}
