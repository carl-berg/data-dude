using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataDude.Instructions.Execute
{
    public class ExecuteInstruction : IInstruction
    {
        public ExecuteInstruction(string sql, object? parameters = null)
        {
            Sql = sql;

            Parameters = new Dictionary<string, object>();

            foreach (var property in parameters?.GetType().GetProperties() ?? Enumerable.Empty<PropertyInfo>())
            {
                Parameters[property.Name] = property.GetValue(parameters, null);
            }
        }

        public string Sql { get; }
        public Dictionary<string, object> Parameters { get; }
    }
}
