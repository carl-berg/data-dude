using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using DataDude.Instructions.Insert;
using DataDude.Schema;

namespace DataDude.Core
{
    public struct DataDudeDbParameter
    {
        public DataDudeDbParameter(ColumnInformation column, ColumnValue? value)
        {
            Column = column;
            ColumnValue = value;
        }

        public ColumnInformation Column { get; }
        public ColumnValue? ColumnValue { get; }

        public void AddParameterTo(DbCommand cmd)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = $"@{Column.Name}";
            param.Value = ColumnValue?.Value ?? DBNull.Value;
            param.Direction = ParameterDirection.Input;
            
            if (ColumnValue?.DbType is { } dbType)
            {
                param.DbType = dbType;
            }

            if (Column is { DataType: "geography" })
            {
                if (param.GetType().GetProperty("UdtTypeName") is PropertyInfo udtProperty)
                {
                    udtProperty.SetValue(param, "geography");
                }

            }

            cmd.Parameters.Add(param);
        }
    }
}
