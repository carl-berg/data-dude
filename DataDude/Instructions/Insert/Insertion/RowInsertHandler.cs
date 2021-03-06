﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.Insertion
{
    public abstract class RowInsertHandler : IInsertRowHandler
    {
        public abstract bool CanHandleInsert(InsertStatement statement, InsertContext context);

        public abstract Task<InsertedRow> Insert(InsertStatement statement, InsertContext context, IDbConnection connection, IDbTransaction? transaction = null);

        protected (string columns, string values, DynamicParameters parameters) GetInsertInformation(InsertStatement statement)
        {
            var columnsToInsert = statement.Data.Where(x => x.Value.Type == ColumnValueType.Set);
            var columns = string.Join(", ", columnsToInsert.Select(x => $"[{x.Column.Name}]"));
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
