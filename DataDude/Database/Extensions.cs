using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataDude.Core
{
    internal static class Extensions
    {
        internal static async ValueTask<int> ExecuteAsync(this DbConnection connection, string statement, DbTransaction? transaction)
        {
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = statement;
            return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        internal static async ValueTask<IReadOnlyDictionary<string, object>> QuerySingleAsync(this DbConnection connection, string statement, IReadOnlyList<DataDudeDbParameter>? parameters = null, DbTransaction ? transaction = null)
        {
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = statement;

            foreach (var parameter in parameters ?? Array.Empty<DataDudeDbParameter>())
            {
                parameter.AddParameterTo(cmd);
            }

            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false);
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
