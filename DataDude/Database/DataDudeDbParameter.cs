using System;
using System.Data;
using System.Data.Common;

namespace DataDude.Core
{
    public struct DataDudeDbParameter
    {
        public DataDudeDbParameter(string name, object? value, DbType? dbType)
        {
            Name = name;
            Value = value;
            DbType = dbType;
        }

        public string Name { get; }
        public object? Value { get; }
        public DbType? DbType { get; }

        public void AddParameterTo(DbCommand cmd)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = $"@{Name}";
            param.Value = Value ?? DBNull.Value;
            param.Direction = ParameterDirection.Input;
            if (DbType is { })
            {
                param.DbType = DbType.Value;
            }

            cmd.Parameters.Add(param);
        }
    }
}
