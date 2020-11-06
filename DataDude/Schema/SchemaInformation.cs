using Dapper;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataDude.Schema
{
    public class SchemaInformation : IEnumerable<TableInformation>
    {
        private IDictionary<string, TableInformation> _tables;
        public SchemaInformation(IEnumerable<TableInformation> tables) 
        {
            _tables = tables.ToDictionary(x => $"{x.Schema}.{x.Name}", x => x);
        }
        public TableInformation? this[string tableName]
        {
            get
            {
                if (_tables.TryGetValue(tableName, out var schemaMatch))
                {
                    return schemaMatch;
                }

                var tableMatch = _tables.Values.Where(x => x.Name == tableName).ToList();
                if (tableMatch.Count == 1)
                {
                    return tableMatch.Single();
                }
                else if (tableMatch.Count > 1)
                {
                    throw new SchemaInformationException($"Found multiple tables matching '{tableName}'");
                }

                return null;
            }
        }
        public static async Task<SchemaInformation> Load(IDbConnection connection, IDbTransaction? transaction = null)
        {
            using var reader = await connection.QueryMultipleAsync(
                @"SELECT 
					s.name as TableSchema,
					t.name as TableName,
					c.name as ColumnName,
					y.name as DataType,
					c.is_nullable As IsNullable,
					c.is_identity as IsIdentity,
					c.is_computed as IsComputed,
					IIF(c.default_object_id > 0, 1, 0) as HasDefault,
					c.max_length AS MaxLength,
					c.precision as Precision,
					c.Scale as Scale
				FROM sys.tables t
				inner join sys.columns c on c.object_id = t.object_id
				inner join sys.types y ON c.user_type_id = y.user_type_id
				inner join sys.schemas s ON s.schema_id = t.schema_id
				ORDER BY s.name, t.name

                SELECT 
	                OBJECT_NAME(f.object_id) as ConstraintName,
	                SCHEMA_NAME(f.schema_id) AS SchemaName,
                    OBJECT_NAME(f.parent_object_id) TableName,
                    COL_NAME(fk.parent_object_id,fk.parent_column_id) ColumnName,
	                SCHEMA_NAME(referenced_table.schema_id) AS ReferencedSchemaName,
                    OBJECT_NAME(fk.referenced_object_id) as ReferencedTableName,
                    COL_NAME(fk.referenced_object_id, fk.referenced_column_id) as ReferencedColumnName
                FROM sys.foreign_keys AS f
                INNER JOIN sys.foreign_key_columns AS fk ON f.object_id = fk.constraint_object_id
                INNER JOIN sys.tables referenced_table ON fk.referenced_object_id = referenced_table.object_id",
                transaction: transaction);

            var allColumns = await reader.ReadAsync<SysColumns>();
            var allForeignKeys = await reader.ReadAsync<ForeignKey>();

            var tables = allColumns
                .GroupBy(x => (x.TableSchema, x.TableName))
                .Select(table => new TableInformation(
                    table.Key.TableSchema,
                    table.Key.TableName,
                    table.Select(x => new ColumnInformation(
                        x.ColumnName,
                        x.DataType,
                        x.IsIdentity,
                        x.IsNullable,
                        x.IsComputed,
                        x.HasDefault,
                        x.MaxLength,
                        x.Precision,
                        x.Scale))));

            var schema = new SchemaInformation(tables);

            foreach (var key in allForeignKeys)
            {
                var column = schema[$"{key.SchemaName}.{key.TableName}"]?[key.ColumnName];
                var referenceTable = schema[$"{key.ReferencedSchemaName}.{key.ReferencedTableName}"];
                if (column is { } && referenceTable is { } && referenceTable[key.ReferencedColumnName] is { } referenceColumn)
                {
                    column.AddForeignKey(new ForeignKeyInformation(key.ConstraintName, referenceTable, referenceColumn));
                }
                else
                {
                    throw new SchemaInformationException($"Failed to match schema columns for foreign key {key.ConstraintName}");
                }
            }

            return schema;
        }
        public IEnumerator<TableInformation> GetEnumerator() => _tables.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class SysColumns
        {
            public string TableSchema { get; set; } = default!;
            public string TableName { get; set; } = default!;
            public string ColumnName { get; set; } = default!;
            public string DataType { get; set; } = default!;
            public bool IsNullable { get; set; }
            public bool IsIdentity { get; set; }
            public bool IsComputed { get; set; }
            public bool HasDefault { get; set; }
            public int MaxLength { get; set; }
            public int Precision { get; set; }
            public int Scale { get; set; }
        }

        private class ForeignKey
        {
            public string ConstraintName { get; set; } = default!;
            public string SchemaName { get; set; } = default!;
            public string TableName { get; set; } = default!;
            public string ColumnName { get; set; } = default!;
            public string ReferencedSchemaName { get; set; } = default!;
            public string ReferencedTableName { get; set; } = default!;
            public string ReferencedColumnName { get; set; } = default!;
        }
    }
}
