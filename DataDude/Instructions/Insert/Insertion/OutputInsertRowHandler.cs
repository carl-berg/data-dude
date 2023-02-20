using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using DataDude.Core;

namespace DataDude.Instructions.Insert.Insertion
{
    /// <summary>
    /// Insert handler that uses OUTPUT inserted.* to retrieve the inserted row.
    /// This strategy requires DDL triggers on the table to be disabled before the insert,
    /// therefore this insert handler will disable all table triggers before the insert and enable them afterwards.
    /// </summary>
    public class OutputInsertRowHandler : RowInsertHandler
    {
        public override bool CanHandleInsert(InsertStatement statement, InsertContext context) => true;

        public override async ValueTask<InsertedRow> Insert(InsertStatement statement, InsertContext context, DbConnection connection, DbTransaction? transaction = null)
        {
            var (columns, values, parameters) = GetInsertInformation(statement);
            await statement.Table.DisableTriggers(connection, transaction);
            var insertedRow = await connection.QuerySingleAsync(
                $@"INSERT INTO {statement.Table.FullName}({columns}) OUTPUT inserted.* VALUES({values})",
                parameters,
                transaction);
            await statement.Table.EnableTriggers(connection, transaction);

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
