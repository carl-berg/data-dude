using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Instructions.Insert.Insertion;

namespace DataDude.Instructions.Insert.Interception
{
    /// <summary>
    /// This interceptor is needed as long as we use OUTPUT inserted.* to get inserted row data.
    /// </summary>
    public class DisableTableTriggersInsertInterceptor : IInsertInterceptor
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

        public async Task OnInserted(InsertedRow insertedRow, InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
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

        public virtual bool ShouldBeInvoked(InsertStatement statement, IInsertRowHandler handler)
        {
            return handler is OutputInsertRowHandler && statement.Table.Triggers.Any(x => !x.IsDisabled);
        }
    }
}
