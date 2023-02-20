using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Core;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.Insertion
{
    public abstract class RowInsertHandler : IInsertRowHandler
    {
        public abstract bool CanHandleInsert(InsertStatement statement, InsertContext context);

        public abstract ValueTask<InsertedRow> Insert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null);

        protected internal (string columns, string values, IReadOnlyList<DataDudeDbParameter> parameters) GetInsertInformation(InsertStatement statement)
        {
            var columnsToInsert = statement.Data.Where(x => x.Value.Type == ColumnValueType.Set);
            var columns = string.Join(", ", columnsToInsert.Select(x => $"[{x.Column.Name}]"));
            var values = string.Join(", ", columnsToInsert.Select(x => GetParameterNameOrRawSql(x.Column, x.Value)));
            var parameters = new List<DataDudeDbParameter>();
            foreach (var (column, value) in columnsToInsert)
            {
                if (!(value.Value is RawSql))
                {
                    parameters.Add(new (column, value));
                }
            }

            return (columns, values, parameters);
        }

        protected string GetParameterNameOrRawSql(ColumnInformation column, ColumnValue columnValue)
        {
            if (columnValue.Value is RawSql rawSql)
            {
                return rawSql.ToString();
            }

            return $"@{column.Name}";
        }

        protected InsertedRow MakeInsertedRow(InsertStatement statement, object insertedRow)
        {
            if (insertedRow is IReadOnlyDictionary<string, object> { } typedRow)
            {
                return new InsertedRow(statement.Table, typedRow, this);
            }
            else
            {
                throw new InsertHandlerException("Could not parse inserted row as dictionary", statement: statement);
            }
        }
    }
}
