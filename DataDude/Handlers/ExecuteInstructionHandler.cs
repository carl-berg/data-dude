using System.Data;
using System.Threading.Tasks;
using Dapper;
using DataDude.Instructions;

namespace DataDude.Handlers
{
    public class ExecuteInstructionHandler : IDataDudeInstructionHandler
    {
        public async Task<DataDudeInstructionHandlerResult> Handle(IDataDudeInstruction instruction, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (instruction is ExecuteInstruction executeInstruction)
            {
                await connection.ExecuteAsync(executeInstruction.Sql, executeInstruction.Parameters, transaction);
                return new DataDudeInstructionHandlerResult(true);
            }

            return new DataDudeInstructionHandlerResult(false);
        }

        public Task PreProcess(IDataDudeInstruction instruction, DataDudeContext context) => Task.CompletedTask;
    }
}
