using System.Collections.Generic;
using DataDude.Instructions;
using DataDude.Instructions.Execute;
using DataDude.Instructions.Insert;
using DataDude.Instructions.Insert.Insertion;
using DataDude.Instructions.Insert.Interception;
using DataDude.Instructions.Insert.ValueProviders;
using DataDude.Schema;

namespace DataDude
{
    public class DataDudeContext
    {
        private Dictionary<string, object> _store;
        public DataDudeContext() => _store = new Dictionary<string, object>();

        public IList<IInstruction> Instructions { get; } = new List<IInstruction>();

        public IList<IInstructionHandler> InstructionHandlers { get; } = new List<IInstructionHandler>
        {
            new ExecuteInstructionHandler(),
            new InsertInstructionHandler(),
        };

        public IList<IInsertInterceptor> InsertInterceptors { get; } = new List<IInsertInterceptor>
        {
            new DisableTableTriggersInsertInterceptor(),
            new IndentityInsertInterceptor(),
        };

        public IList<IInsertValueProvider> InsertValueProviders { get; } = new List<IInsertValueProvider>
        {
            new StringValueProvider(),
            new NumericValueProvider(),
            new BinaryValueProvider(),
            new DateValueProvider(),
            new BoolValueProvider(),
            new VersionValueProvider(),
        };

        public IList<IInsertRowHandler> InsertRowHandlers { get; } = new List<IInsertRowHandler>
        {
            new IdentityInsertRowHandler(),
            new GeneratingInsertRowHandler(),
            new OutputInsertRowHandler(),
        };

        public IList<InsertedRow> InsertedRows { get; } = new List<InsertedRow>();

        public SchemaInformation? Schema { get; internal set; }

        public UniqueValueGenerator PrimaryKeyValueGenerator { get; set; }

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
