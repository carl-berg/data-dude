using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DataDude.Instructions;
using DataDude.Schema;

[assembly: InternalsVisibleTo("DataDude.Tests")]

namespace DataDude
{
    public class Dude
    {
        public Dude(ISchemaLoader? schemaLoader = null)
        {
            Context = new DataDudeContext(schemaLoader ?? new SqlServer.SqlServerSchemaLoader());
        }

        protected DataDudeContext Context { get; }

        public void Configure(Action<DataDudeContext> configure) => configure?.Invoke(Context);

        public async Task Go(IDbConnection connection, IDbTransaction? transaction = null)
        {
            await Context.LoadSchema(connection, transaction);

            foreach (var preProcessor in Context.InstructionPreProcessors)
            {
                await preProcessor.PreProcess(Context);
            }

            foreach (var instruction in Context.Instructions)
            {
                bool wasHandled = false;
                foreach (var handler in Context.InstructionHandlers)
                {
                    try
                    {
                        var result = await handler.Handle(instruction, Context, connection, transaction);
                        if (result is { Handled: true })
                        {
                            wasHandled = true;
                        }
                    }
                    catch (Exception ex) when (ex is not HandlerException)
                    {
                        throw new HandlerException($"Data dude failed while processing an instruction of type '{instruction.GetType()}'", ex);
                    }
                }

                if (!wasHandled)
                {
                    throw new HandlerException($"Instruction of type '{instruction.GetType()}' lacks handler and cannot be processed");
                }
            }

            Context.Instructions.Clear();
        }
    }
}
