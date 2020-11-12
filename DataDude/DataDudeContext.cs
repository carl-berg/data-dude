using System.Collections.Generic;
using DataDude.Handlers;
using DataDude.Handlers.Insert;
using DataDude.Handlers.Insert.DefaultValueHandlers;
using DataDude.Handlers.Insert.Interception;
using DataDude.Instructions;
using DataDude.Schema;

namespace DataDude
{
    public class DataDudeContext
    {
        private Dictionary<string, object> _store;
        public DataDudeContext() => _store = new Dictionary<string, object>();

        public IList<IDataDudeInstruction> Instructions { get; } = new List<IDataDudeInstruction>();

        public IList<IDataDudeInstructionHandler> InstructionHandlers { get; } = new List<IDataDudeInstructionHandler>
        {
            new ExecuteInstructionHandler(),
            new InsertInstructionHandler(),
        };

        public IList<IDataDudeInsertInterceptor> InsertInterceptors { get; } = new List<IDataDudeInsertInterceptor>
        {
            new DisableTableTriggersInsertInterceptor(),
            new IndentityInsertInterceptor(),
        };

        public IList<IDataDudeInsertValueHandler> InsertValueHandlers { get; } = new List<IDataDudeInsertValueHandler>
        {
            new StringDefaultValueHandler(),
            new NumericDefaultValueHandler(),
            new BinaryDefaultHandler(),
            new DateDefaultValueHandler(),
            new BoolDefaultHandler(),
            new VersionDefaultHandler(),
        };

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
