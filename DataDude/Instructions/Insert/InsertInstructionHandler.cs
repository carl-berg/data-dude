using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Instructions.Insert.Insertion;
using DataDude.Schema;

namespace DataDude.Instructions.Insert
{
    public class InsertInstructionHandler : IInstructionHandler
    {
        public virtual async Task<HandleInstructionResult> Handle(IInstruction instruction, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (instruction is InsertInstruction insert)
            {
                if (context.Schema?[insert.TableName] is { } table)
                {
                    var statement = new InsertStatement(table, insert);

                    try
                    {
                        foreach (var valueHandler in context.InsertValueProviders)
                        {
                            statement.InvokeValueProvider(valueHandler);
                        }

                        if (context.InsertRowHandlers.FirstOrDefault(x => x.CanHandleInsert(statement)) is IInsertRowHandler handler)
                        {
                            await handler.PreProcessStatement(statement, connection, transaction);

                            foreach (var insertInterceptor in context.InsertInterceptors)
                            {
                                await insertInterceptor.OnInsert(statement, context, connection, transaction);
                            }

                            var insertedRow = await handler.Insert(statement, connection, transaction);

                            foreach (var insertInterceptor in context.InsertInterceptors)
                            {
                                await insertInterceptor.OnInserted(insertedRow, statement, context, connection, transaction);
                            }
                        }
                        else
                        {
                            throw new HandlerException($"Could not resolve a row handler for insertion of a row in {insert.TableName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new HandlerException($"Insertion into table {insert.TableName} failed", ex);
                    }
                }
                else
                {
                    throw new HandlerException($"Could not find table {insert.TableName} in schema");
                }

                return new HandleInstructionResult(true);
            }

            return new HandleInstructionResult(false);
        }
    }
}
