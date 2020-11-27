﻿using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataDude.Instructions.Insert.Interception
{
    /// <summary>
    /// Attempts to set fk columns based on last previous inserted row if possible.
    /// </summary>
    public class ForeignKeyInterceptor : IInsertInterceptor
    {
        public Task OnInsert(InsertStatement statement, InsertContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            foreach (var fk in statement.Table.ForeignKeys)
            {
                if (context.InsertedRows.Where(x => x.Table == fk.ReferencedTable).LastOrDefault() is { } lastInsert)
                {
                    foreach (var (insertValue, referencedColumn) in fk.Columns.Select(c => (statement.Data[c.Column], c.ReferencedColumn)))
                    {
                        insertValue.Set(new ColumnValue(lastInsert[referencedColumn.Name]));
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task OnInserted(
            InsertedRow insertedRow,
            InsertStatement statement,
            InsertContext context,
            IDbConnection connection,
            IDbTransaction? transaction = null) => Task.CompletedTask;
    }
}
