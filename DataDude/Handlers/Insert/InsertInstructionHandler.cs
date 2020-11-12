using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Instructions;

namespace DataDude.Handlers.Insert
{
    public class InsertInstructionHandler : IDataDudeInstructionHandler
    {
        public virtual async Task<DataDudeInstructionHandlerResult> Handle(IDataDudeInstruction instruction, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (instruction is InsertInstruction insert)
            {
                if (context.Schema[insert.TableName] is { } table)
                {
                    var statement = new InsertStatement(table, insert);

                    foreach (var valueHandler in context.InsertValueHandlers)
                    {
                        statement.InvokeValueHandler(valueHandler);
                    }

                    foreach (var insertInterceptor in context.InsertInterceptors)
                    {
                        await insertInterceptor.OnInsert(statement, context, connection, transaction);
                    }

                    var insertedRow = await Insert(statement, connection, transaction);

                    foreach (var insertInterceptor in context.InsertInterceptors)
                    {
                        await insertInterceptor.OnInserted(insertedRow, statement, context, connection, transaction);
                    }
                }
                else
                {
                    throw new System.Exception($"Could not find table {insert.TableName} in schema");
                }

                return new DataDudeInstructionHandlerResult(true);
            }

            return new DataDudeInstructionHandlerResult(false);
        }

        protected virtual async Task<IDictionary<string, object>> Insert(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var columnsToInsert = statement.Where(x => x.Value.Type == ColumnValueType.Set);
            var columns = string.Join(", ", columnsToInsert.Select(x => x.Key.Name));
            var values = string.Join(", ", columnsToInsert.Select(x => $"@{x.Key.Name}"));
            var parameters = new DynamicParameters();
            foreach (var p in columnsToInsert)
            {
                parameters.Add(p.Key.Name, p.Value.Value, p.Value.DbType);
            }

            // How do we best handle fetching inserted row? This won't work if there are triggers on the table, they will need to be disabled during this execution
            // which is feasible, but maybe not expected?
            var insertedRow = await connection.QuerySingleAsync<dynamic>(
                $@"INSERT INTO {statement.Table.Schema}.{statement.Table.Name}({columns}) OUTPUT inserted.* VALUES({values})",
                parameters,
                transaction);

            return insertedRow as IDictionary<string, object>;
        }
    }
}
