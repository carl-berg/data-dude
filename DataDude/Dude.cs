using DataDude.Instructions;
using System.Data.Common;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DataDude.Tests")]

namespace DataDude
{
    public class Dude
    {
        public Dude(ISchemaLoader? schemaLoader = null, Action<DataDudeContext>? configure = null)
        {
            Context = new DataDudeContext(schemaLoader ?? new SqlServer.SqlServerSchemaLoader());
            configure?.Invoke(Context);
        }

        protected DataDudeContext Context { get; }

        public Dude Configure(Action<DataDudeContext> configure)
        {
            configure.Invoke(Context);
            return this;
        }

        public async ValueTask Go(DbConnection connection, DbTransaction? transaction = null)
        {
            foreach (var preProcessor in Context.InstructionDecorators)
            {
                await preProcessor.Initialize(Context);
            }

            await Context.LoadSchema(connection, transaction);

            foreach (var preProcessor in Context.InstructionDecorators)
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

            foreach (var preProcessor in Context.InstructionDecorators.Reverse())
            {
                await preProcessor.PostProcess(Context);
            }

            Context.Instructions.Clear();
        }
    }
}
