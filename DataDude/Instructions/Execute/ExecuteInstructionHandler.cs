using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace DataDude.Instructions.Execute
{
    public class ExecuteInstructionHandler : IInstructionHandler
    {
        public async Task<HandleInstructionResult> Handle(IInstruction instruction, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (instruction is ExecuteInstruction executeInstruction)
            {
                await connection.ExecuteAsync(executeInstruction.Sql, executeInstruction.Parameters, transaction);
                return new HandleInstructionResult(true);
            }

            return new HandleInstructionResult(false);
        }
    }
}
