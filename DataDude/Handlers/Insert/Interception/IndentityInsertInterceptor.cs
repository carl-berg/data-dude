﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DataDude.Handlers.Insert.Interception
{
    public class IndentityInsertInterceptor : IDataDudeInsertInterceptor
    {
        public async Task OnInsert(InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (statement.Any(x => x.Value.Type == ColumnValueType.Set && x.Key.IsIdentity))
            {
                await connection.ExecuteAsync($"SET IDENTITY_INSERT {statement.Table.Schema}.{statement.Table.Name} ON", transaction: transaction);
            }
        }

        public async Task OnInserted(IDictionary<string, object> insertedRow, InsertStatement statement, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (statement.Any(x => x.Value.Type == ColumnValueType.Set && x.Key.IsIdentity))
            {
                await connection.ExecuteAsync($"SET IDENTITY_INSERT {statement.Table.Schema}.{statement.Table.Name} OFF", transaction: transaction);
            }
        }
    }
}
