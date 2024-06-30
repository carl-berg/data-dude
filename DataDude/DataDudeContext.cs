using System.Data;
using System.Data.Common;
using DataDude.Instructions;
using DataDude.Instructions.Execute;
using DataDude.Instructions.Insert;
using DataDude.Schema;

namespace DataDude
{
    public class DataDudeContext
    {
        internal const string SchemaKey = "Schema";
        private Dictionary<string, object> _store = [];
        
        public DataDudeContext(ISchemaLoader schemaLoader)
        {
            SchemaLoader = schemaLoader;
            InstructionHandlers = [new ExecuteInstructionHandler(), new InsertInstructionHandler(this)];
            InstructionDecorators = [new StaticCache()];
        }

        public static IDictionary<string, DbType> TypeMappings { get; } = new Dictionary<string, DbType>
        {
            ["bit"] = DbType.Boolean,
            ["bigint"] = DbType.Int64,
            ["binary"] = DbType.Binary,
            ["char"] = DbType.String,
            ["date"] = DbType.Date,
            ["datetime"] = DbType.DateTime,
            ["datetime2"] = DbType.DateTime2,
            ["datetimeoffset"] = DbType.DateTimeOffset,
            ["decimal"] = DbType.Decimal,
            ["float"] = DbType.Double,
            ["geography"] = DbType.String,
            ["int"] = DbType.Int32,
            ["image"] = DbType.Binary,
            ["nchar"] = DbType.String,
            ["ntext"] = DbType.String,
            ["nvarchar"] = DbType.String,
            ["numeric"] = DbType.Decimal,
            ["money"] = DbType.Decimal,
            ["real"] = DbType.Double,
            ["smalldatetime"] = DbType.DateTime,
            ["smallint"] = DbType.Int16,
            ["smallmoney"] = DbType.Decimal,
            ["text"] = DbType.String,
            ["tinyint"] = DbType.Int16,
            ["time"] = DbType.Time,
            ["timestamp"] = DbType.Binary,
            ["uniqueidentifier"] = DbType.Guid,
            ["varbinary"] = DbType.Binary,
            ["varchar"] = DbType.String,
            ["variant"] = DbType.Object,
        };

        public ISchemaLoader SchemaLoader { get; }

        public IList<IInstruction> Instructions { get; } = [];

        public IList<IInstructionHandler> InstructionHandlers { get; }

        public IList<IInstructionDecorator> InstructionDecorators { get; }

        public IEnumerable<string> ContextKeys => _store.Keys;

        public SchemaInformation? Schema => Get<SchemaInformation>(SchemaKey);

        public static DbType GetDbType(ColumnInformation column)
        {
            if (TypeMappings.ContainsKey(column.DataType))
            {
                return TypeMappings[column.DataType];
            }

            throw new NotImplementedException(
                $"""
                Db type for {column.DataType} of column {column.Table.FullName}.{column.Name} is not known.
                You can update DataDudeContext.TypeMappings with missing mappings to correct missing defaults.
                """);
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

        public async ValueTask LoadSchema(DbConnection connection, DbTransaction? transaction = null)
        {
            if (_store.ContainsKey(SchemaKey) is false)
            {
                Console.WriteLine("Loading schema");
                var schema = await SchemaLoader.Load(connection, transaction);
                Set(SchemaKey, schema);
            }
            else
            {
                Console.WriteLine("Schema already loaded");
            }
        }
    }
}
