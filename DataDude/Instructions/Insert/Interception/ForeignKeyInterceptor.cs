using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Instructions.Insert.Insertion;

namespace DataDude.Instructions.Insert.Interception
{
    public class ForeignKeyInterceptor : IInsertInterceptor
    {
        public Task OnInsert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
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
            DataDudeContext context,
            IDbConnection connection,
            IDbTransaction? transaction = null) => Task.CompletedTask;

        public virtual bool ShouldBeInvoked(InsertStatement statement, IInsertRowHandler handler) => statement.Table.ForeignKeys.Any();
    }
}
