using System;
using System.Data;
using System.Threading.Tasks;
using DataDude.Instructions;
using DataDude.SqlServer;

namespace DataDude
{
    public class DataDude
    {
        private readonly ISchemaLoader _schemaLoader;

        public DataDude(ISchemaLoader? schemaLoader = null)
        {
            Context = new DataDudeContext();
            _schemaLoader = schemaLoader ?? new SqlServerSchemaLoader();
        }

        protected DataDudeContext Context { get; }

        public void Configure(Action<DataDudeContext> configure) => configure?.Invoke(Context);

        public async Task Go(IDbConnection connection, IDbTransaction? transaction = null)
        {
            Context.Schema = await _schemaLoader.Load(connection, transaction);
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
                    catch (Exception ex)
                    {
                        throw new HandlerException($"Data dude failed while processing an instruction of type '{instruction.GetType()}'", ex);
                    }
                }

                if (!wasHandled)
                {
                    throw new HandlerException($"Instruction of type '{instruction.GetType()}' lacks handler and cannot be processed");
                }
            }
        }
    }
}
