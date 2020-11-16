using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.Insertion
{
    public abstract class RowInsertHandler : IInsertRowHandler
    {
        public abstract bool CanHandleInsert(InsertStatement statement);

        public abstract Task<InsertedRow> Insert(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null);

        public virtual Task PreProcessStatement(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null) => Task.CompletedTask;

        protected (string columns, string values, DynamicParameters parameters) GetInsertInformation(InsertStatement statement)
        {
            var columnsToInsert = statement.Data.Where(x => x.Value.Type == ColumnValueType.Set);
            var columns = string.Join(", ", columnsToInsert.Select(x => x.Column.Name));
            var values = string.Join(", ", columnsToInsert.Select(x => GetParameterNameOrRawSql(x.Column, x.Value)));
            var parameters = new DynamicParameters();
            foreach (var (column, value) in columnsToInsert)
            {
                if (!(value.Value is RawSql))
                {
                    parameters.Add(column.Name, value.Value, value.DbType);
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
    }
}
