using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Instructions.Insert.Interception;

namespace DataDude.Instructions.Insert.AutomaticForeignKeys
{
    /// <summary>
    /// Attempts to set fk columns based on last previous inserted row if possible.
    /// </summary>
    public class ForeignKeyInterceptor : IInsertInterceptor
    {
        public ValueTask OnInsert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null)
        {
            foreach (var fk in statement.Table.ForeignKeys)
            {
                if (context.InsertedRows.Where(x => x.Table == fk.ReferencedTable).LastOrDefault() is { } lastInsert)
                {
                    foreach (var (insertValue, referencedColumn) in fk.Columns.Select(c => (statement.Data[c.Column], c.ReferencedColumn)))
                    {
                        if (insertValue.Type == ColumnValueType.NotSet)
                        {
                            insertValue.Set(new ColumnValue(lastInsert[referencedColumn.Name]));
                        }
                    }
                }
            }

            return default;
        }

        public ValueTask OnInserted(
            InsertedRow insertedRow,
            InsertStatement statement,
            InsertContext context,
            DbConnection connection,
            DbTransaction? transaction = null) => default;
    }
}
