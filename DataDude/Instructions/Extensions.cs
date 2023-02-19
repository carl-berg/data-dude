using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Core;
using DataDude.Schema;

namespace DataDude.Instructions
{
    internal static class Extensions
    {
        internal static async Task DisableTriggers(this TableInformation table, DbConnection connection, DbTransaction? transaction = null)
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

        internal static async Task EnableTriggers(this TableInformation table, DbConnection connection, DbTransaction? transaction = null)
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
