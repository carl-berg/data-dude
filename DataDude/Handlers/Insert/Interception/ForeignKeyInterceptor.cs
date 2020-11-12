using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataDude.Handlers.Insert.Interception
{
    public class ForeignKeyInterceptor : IDataDudeInsertInterceptor
    {
        public Task OnInsert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            foreach (var fk in statement.Table.ForeignKeys)
            {
                var key = $"Inserted_{fk.ReferencedTable.Schema}.{fk.ReferencedTable.Name}";
                if (context.Get<IList<IDictionary<string, object>>>(key) is { } inserted && inserted.LastOrDefault() is { } lastInsert)
                {
                    foreach (var (insertValue, referencedColumn) in fk.Columns.Select(c => (statement[c.Column], c.ReferencedColumn)))
                    {
                        insertValue.Set(new ColumnValue(lastInsert[referencedColumn.Name]));
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task OnInserted(IDictionary<string, object> insertedRow, InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var key = $"Inserted_{statement.Table.Schema}.{statement.Table.Name}";
            var inserted = context.Get<IList<IDictionary<string, object>>>(key) ?? new List<IDictionary<string, object>>();
            inserted.Add(insertedRow);
            context.Set(key, inserted);
            return Task.CompletedTask;
        }
    }
}
