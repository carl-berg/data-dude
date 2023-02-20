using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataDude.Instructions.Execute
{
    public class ExecuteInstructionHandler : IInstructionHandler
    {
        public async ValueTask<HandleInstructionResult> Handle(IInstruction instruction, DataDudeContext context, DbConnection connection, DbTransaction? transaction = null)
        {
            if (instruction is ExecuteInstruction executeInstruction)
            {
                var command = connection.CreateCommand();

                command.CommandText = executeInstruction.Sql;
                foreach (var instructionParameter in executeInstruction.Parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = instructionParameter.Key;
                    parameter.Value = instructionParameter.Value;
                    command.Parameters.Add(parameter);
                }

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                return new HandleInstructionResult(true);
            }

            return new HandleInstructionResult(false);
        }
    }
}
