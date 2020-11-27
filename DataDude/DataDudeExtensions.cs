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
            dude.Configure(x => InsertContext.Get(x)?.InsertInterceptors.Add(new ForeignKeyInterceptor()));
            return dude;
        }

        public static async Task DisableTriggers(this TableInformation table, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var disableTriggerStatements = table.Triggers
                .Where(x => !x.IsDisabled)
                .Select(x => $"DISABLE TRIGGER {x.Name} ON {table.Schema}.{table.Name}");
            if (disableTriggerStatements.Any())
            {
                var sql = string.Join(Environment.NewLine, disableTriggerStatements);
                await connection.ExecuteAsync(sql, transaction: transaction);
            }
        }

        public static async Task EnableTriggers(this TableInformation table, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var enableTriggerStatements = table.Triggers
                .Where(x => !x.IsDisabled)
                .Select(x => $"ENABLE TRIGGER {x.Name} ON {table.Schema}.{table.Name}");
            if (enableTriggerStatements.Any())
            {
                var sql = string.Join(Environment.NewLine, enableTriggerStatements);
                await connection.ExecuteAsync(sql, transaction: transaction);
            }
        }
    }
}
