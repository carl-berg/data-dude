﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Interception
{
    public class ForeignKeyInterceptor : IInsertInterceptor
    {
        public Task OnInsert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            foreach (var fk in statement.Table.ForeignKeys)
            {
                var key = $"Inserted_{fk.ReferencedTable.Schema}.{fk.ReferencedTable.Name}";
                if (context.Get<IList<InsertedRow>>(key) is { } inserted && inserted.LastOrDefault() is { } lastInsert)
                {
                    foreach (var (insertValue, referencedColumn) in fk.Columns.Select(c => (statement.Data[c.Column], c.ReferencedColumn)))
                    {
                        insertValue.Set(new ColumnValue(lastInsert[referencedColumn.Name]));
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task OnInserted(InsertedRow insertedRow, InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var key = $"Inserted_{statement.Table.Schema}.{statement.Table.Name}";
            var inserted = context.Get<IList<InsertedRow>>(key) ?? new List<InsertedRow>();
            inserted.Add(insertedRow);
            context.Set(key, inserted);
            return Task.CompletedTask;
        }
    }
}
