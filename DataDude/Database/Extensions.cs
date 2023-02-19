using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataDude.Core
{
    internal static class Extensions
    {
        internal static Task<int> ExecuteAsync(this DbConnection connection, string statement, DbTransaction? transaction)
        {
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = statement;
            return cmd.ExecuteNonQueryAsync();
        }

        internal static async Task<IReadOnlyDictionary<string, object>> QuerySingleAsync(this DbConnection connection, string statement, IReadOnlyList<DataDudeDbParameter>? parameters = null, DbTransaction ? transaction = null)
        {
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = statement;

            foreach (var parameter in parameters ?? Array.Empty<DataDudeDbParameter>())
            {
                parameter.AddParameterTo(cmd);
            }

            using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (reader.CanGetColumnSchema())
            {
                var columns = reader.GetColumnSchema();
                await reader.ReadAsync();
                var record = new Dictionary<string, object>();
                
                foreach (var column in columns)
                {
                    if (column.ColumnOrdinal is { } ordinal)
                    {
                        record[column.ColumnName] = reader.GetValue(ordinal);
                    }
                }

                return record;
            }

            throw new Exception($"Cannot extract reader schema from query{Environment.NewLine}{statement}");
        }

        
    }
}
