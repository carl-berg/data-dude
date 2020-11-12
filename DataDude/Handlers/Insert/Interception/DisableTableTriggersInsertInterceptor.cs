using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DataDude.Handlers.Insert.Interception
{
    /// <summary>
    /// This interceptor is needed as long as we use OUTPUT inserted.* to get inserted row data
    /// </summary>
    public class DisableTableTriggersInsertInterceptor : IDataDudeInsertInterceptor
    {
        public async Task OnInsert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var disableTriggerStatements = statement.Table.Triggers
                .Where(x => !x.IsDisabled)
                .Select(x => $"DISABLE TRIGGER {x.Name} ON {statement.Table.Schema}.{statement.Table.Name}");
            if (disableTriggerStatements.Any())
            {
                var sql = string.Join(Environment.NewLine, disableTriggerStatements);
                await connection.ExecuteAsync(sql, transaction: transaction);
            }
        }

        public async Task OnInserted(IDictionary<string, object> insertedRow, InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var enableTriggerStatements = statement.Table.Triggers
                .Where(x => !x.IsDisabled)
                .Select(x => $"ENABLE TRIGGER {x.Name} ON {statement.Table.Schema}.{statement.Table.Name}");
            if (enableTriggerStatements.Any())
            {
                var sql = string.Join(Environment.NewLine, enableTriggerStatements);
                await connection.ExecuteAsync(sql, transaction: transaction);
            }
        }
    }
}
