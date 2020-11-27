using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Schema;

namespace DataDude.Instructions
{
    public static class Extensions
    {
        public static async Task DisableTriggers(this TableInformation table, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var disableTriggerStatements = table.Triggers
                .Where(x => !x.IsDisabled)
                .Select(x => $"DISABLE TRIGGER {x.Name} ON {table.FullName}");
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
                .Select(x => $"ENABLE TRIGGER {x.Name} ON {table.FullName}");
            if (enableTriggerStatements.Any())
            {
                var sql = string.Join(Environment.NewLine, enableTriggerStatements);
                await connection.ExecuteAsync(sql, transaction: transaction);
            }
        }
    }
}
