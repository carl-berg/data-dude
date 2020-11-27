using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Instructions.Insert.Insertion;

namespace DataDude.Instructions.Insert
{
    public class InsertInstructionHandler : IInstructionHandler
    {
        public InsertInstructionHandler(DataDudeContext context)
        {
            Context = new InsertContext(context);
        }

        private InsertContext Context { get; }

        public virtual async Task<HandleInstructionResult> Handle(IInstruction instruction, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (instruction is InsertInstruction insert)
            {
                if (context.Schema?[insert.TableName] is { } table)
                {
                    var statement = new InsertStatement(table, insert);

                    try
                    {
                        foreach (var valueHandler in Context.InsertValueProviders)
                        {
                            statement.InvokeValueProvider(valueHandler);
                        }

                        foreach (var insertInterceptor in Context.InsertInterceptors)
                        {
                            await insertInterceptor.OnInsert(statement, Context, connection, transaction);
                        }

                        if (Context.InsertRowHandlers.FirstOrDefault(x => x.CanHandleInsert(statement, Context)) is IInsertRowHandler handler)
                        {
                            var insertedRow = await handler.Insert(statement, Context, connection, transaction);
                            Context.InsertedRows.Add(insertedRow);

                            foreach (var insertInterceptor in Context.InsertInterceptors)
                            {
                                await insertInterceptor.OnInserted(insertedRow, statement, Context, connection, transaction);
                            }
                        }
                        else
                        {
                            throw new InsertHandlerException(
                                $"Could not resolve a row handler for insertion of a row in {insert.TableName}",
                                statement: statement);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InsertHandlerException($"Insertion into table {insert.TableName} failed", ex, statement);
                    }
                }
                else
                {
                    throw new InsertHandlerException($"Could not find table {insert.TableName} in schema");
                }

                return new HandleInstructionResult(true);
            }

            return new HandleInstructionResult(false);
        }
    }
}
