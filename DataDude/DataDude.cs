using System;
using System.Data;
using System.Threading.Tasks;
using DataDude.Handlers;
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

        public DataDudeContext Context { get; }

        public void AddInstruction(IDataDudeInstruction instruction) => Context.Instructions.Add(instruction);

        public void AddHandler(IDataDudeInstructionHandler handler) => Context.InstructionHandlers.Add(handler);

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
                        throw new Exception($"Data dude failed during processing of a '{instruction.GetType()}' type instruction", ex);
                    }
                }

                if (!wasHandled)
                {
                    throw new Exception($"Instruction of type '{instruction.GetType()}' lacks handler and cannot be processed");
                }
            }
        }
    }
}
