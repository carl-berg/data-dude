using System;
using System.Collections;
using System.Data;

namespace DataDude.Handlers.Insert.DefaultValueHandlers
{
    // Borrowed from Captain Data
    public static class TypeConverter
    {
        private static readonly ArrayList _dbTypeList = new ArrayList();

        static TypeConverter()
        {
            _dbTypeList.Add(new DbTypeMapEntry(typeof(bool), DbType.Boolean, "bit"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(DateTime), DbType.Date, "date"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, "datetime"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(DateTime), DbType.DateTime2, "datetime2"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(DateTimeOffset), DbType.DateTimeOffset, "datetimeoffset"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(decimal), DbType.Decimal, "decimal"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(decimal), DbType.VarNumeric, "numeric"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(double), DbType.Double, "float"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(Guid), DbType.Guid, "uniqueidentifier"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(short), DbType.Int16, "smallint"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(int), DbType.Int32, "int"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(long), DbType.Int64, "bigint"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(object), DbType.Object, "variant"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(string), DbType.String, "varchar"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(string), DbType.String, "nvarchar"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(byte[]), DbType.Binary, "varbinary"));
            _dbTypeList.Add(new DbTypeMapEntry(typeof(string), DbType.String, "geography"));
        }

        public static Type ToNetType(string sqlDbType)
        {
            var entry = Find(sqlDbType);
            return entry.Type;
        }

        public static DbType ToDbType(string sqlDbType)
        {
            var entry = Find(sqlDbType);
            return entry.DbType;
        }

        private static DbTypeMapEntry Find(string sqlDbType)
        {
            foreach (var t in _dbTypeList)
            {
                var entry = (DbTypeMapEntry)t;
                if (entry.SqlDbType.ToLower() == sqlDbType.ToLower())
                {
                    return (DbTypeMapEntry)entry;
                }
            }

            throw new ApplicationException($"Referenced an unsupported type ({sqlDbType})");
        }

        private struct DbTypeMapEntry
        {
            public readonly Type Type;
            public readonly DbType DbType;
            public readonly string SqlDbType;
            public DbTypeMapEntry(Type type, DbType dbType, string sqlDbType)
            {
                Type = type;
                DbType = dbType;
                SqlDbType = sqlDbType;
            }
        }
    }
}
