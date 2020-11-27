using System.Collections.Generic;
using DataDude.Instructions;
using DataDude.Instructions.Execute;
using DataDude.Instructions.Insert;
using DataDude.Schema;

namespace DataDude
{
    public class DataDudeContext
    {
        private Dictionary<string, object> _store;
        public DataDudeContext()
        {
            _store = new Dictionary<string, object>();
            Instructions = new List<IInstruction>();
            InstructionHandlers = new List<IInstructionHandler>
            {
                new ExecuteInstructionHandler(),
                new InsertInstructionHandler(this),
            };
        }

        public IList<IInstruction> Instructions { get; }

        public IList<IInstructionHandler> InstructionHandlers { get; }

        public SchemaInformation? Schema { get; internal set; }

        public T? Get<T>(string key)
        {
            if (_store.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }

            return default;
        }

        public void Set<T>(string key, T value)
        {
            if (value is { })
            {
                _store[key] = value;
            }
            else if (_store.ContainsKey(key))
            {
                _store.Remove(key);
            }
        }
    }
}
