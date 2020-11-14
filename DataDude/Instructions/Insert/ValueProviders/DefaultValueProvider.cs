using System;
using System.Data;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    public abstract class DefaultValueProvider : IInsertValueProvider
    {
        public void Process(TableInformation table, ColumnInformation column, ColumnValue value)
        {
            if (column.DefaultValue is { Length: > 0 } ||
                column.IsComputed ||
                column.IsIdentity ||
                value.Type is ColumnValueType.Ignore or ColumnValueType.Set)
            {
                return;
            }
            else if (column.IsNullable)
            {
                value.Set(ColumnValue.Null(GetNullDbType(column.DataType)));
            }
            else if (GetDefaultValue(table, column, value) is { } newValue)
            {
                value.Set(newValue);
            }
        }

        protected abstract ColumnValue? GetDefaultValue(TableInformation table, ColumnInformation column, ColumnValue value);

        private DbType GetNullDbType(string dataType) => dataType switch
        {
            "bit" => DbType.Boolean,
            "date" => DbType.Date,
            "datetime" => DbType.DateTime,
            "datetime2" => DbType.DateTime2,
            "datetimeoffset" => DbType.DateTimeOffset,
            "decimal" => DbType.Decimal,
            "numeric" => DbType.VarNumeric,
            "float" => DbType.Double,
            "uniqueidentifier" => DbType.Guid,
            "smallint" => DbType.Int16,
            "int" => DbType.Int32,
            "bigint" => DbType.Int64,
            "variant" => DbType.Object,
            "varbinary" => DbType.Binary,
            "varchar" or "nvarchar" or "geography" => DbType.String,
            _ => throw new NotImplementedException($"Db type for {dataType} is not known"),
        };
    }
}
