using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DataDude.Instructions;
using DataDude.Instructions.Execute;
using DataDude.Instructions.Insert;
using DataDude.Schema;

namespace DataDude
{
    public class DataDudeContext
    {
        private Dictionary<string, object> _store;
        public DataDudeContext(ISchemaLoader schemaLoader)
        {
            _store = new Dictionary<string, object>();
            SchemaLoader = new CachableSchemaLoader(schemaLoader);
            Instructions = new List<IInstruction>();
            InstructionHandlers = new List<IInstructionHandler>
            {
                new ExecuteInstructionHandler(),
                new InsertInstructionHandler(this),
            };
            InstructionPreProcessors = new List<IInstructionPreProcessor>();
        }

        public static IDictionary<string, DbType> TypeMappings { get; } = new Dictionary<string, DbType>
        {
            ["bit"] = DbType.Boolean,
            ["date"] = DbType.Date,
            ["datetime"] = DbType.DateTime,
            ["datetime2"] = DbType.DateTime2,
            ["datetimeoffset"] = DbType.DateTimeOffset,
            ["decimal"] = DbType.Decimal,
            ["numeric"] = DbType.Decimal,
            ["float"] = DbType.Double,
            ["real"] = DbType.Double,
            ["uniqueidentifier"] = DbType.Guid,
            ["smallint"] = DbType.Int16,
            ["int"] = DbType.Int32,
            ["bigint"] = DbType.Int64,
            ["variant"] = DbType.Object,
            ["varbinary"] = DbType.Binary,
            ["varchar"] = DbType.String,
            ["nvarchar"] = DbType.String,
            ["geography"] = DbType.String,
            ["timestamp"] = DbType.Binary,
        };

        public ISchemaLoader SchemaLoader { get; }

        public IList<IInstruction> Instructions { get; }

        public IList<IInstructionHandler> InstructionHandlers { get; }

        public IList<IInstructionPreProcessor> InstructionPreProcessors { get; }

        public SchemaInformation? Schema { get; private set; }

        public static DbType GetDbType(ColumnInformation column)
        {
            if (TypeMappings.ContainsKey(column.DataType))
            {
                return TypeMappings[column.DataType];
            }

            throw new NotImplementedException($"Db type for {column.DataType} of column {column.Table.FullName}.{column.Name} is not known");
        }

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

        public async Task LoadSchema(IDbConnection connection, IDbTransaction? transaction = null)
        {
            Schema = await SchemaLoader.Load(connection, transaction);
        }
    }
}
