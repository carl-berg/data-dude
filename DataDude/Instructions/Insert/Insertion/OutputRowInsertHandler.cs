using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace DataDude.Instructions.Insert.Insertion
{
    /// <summary>
    /// Insert handler that uses OUTPUT inserted.* to retrieve the inserted row.
    /// This requires any DDL triggers on the table to be disabled before the insert,
    /// therefore the insert handler will disable all table triggers before the insert
    /// and enable them afterwards.
    /// </summary>
    public class OutputRowInsertHandler : RowInsertHandler
    {
        public override Task<bool> CanHandleInsert(InsertStatement statement) => Task.FromResult(true);

        public override async Task<IDictionary<string, object>> Insert(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var (columns, values, parameters) = GetInsertInformation(statement);
            var insertedRow = await connection.QuerySingleAsync<dynamic>(
                $@"INSERT INTO {statement.Table.Schema}.{statement.Table.Name}({columns}) OUTPUT inserted.* VALUES({values})",
                parameters,
                transaction);

            if (insertedRow is IDictionary<string, object> { } typedRow)
            {
                return typedRow;
            }
            else
            {
                throw new HandlerException("Could not parse inserted row as dictionary");
            }
        }
    }
}
