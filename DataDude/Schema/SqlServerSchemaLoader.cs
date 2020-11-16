using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Schema;

namespace DataDude.SqlServer
{
    public class SqlServerSchemaLoader : ISchemaLoader
    {
        public async Task<SchemaInformation> Load(IDbConnection connection, IDbTransaction? transaction = null)
        {
            var (columns, foreignKeys, triggers) = await LoadSchema(connection, transaction);

            var tables = columns
                .GroupBy(x => (x.TableSchema, x.TableName))
                .Select(table => new TableInformation(
                    table.Key.TableSchema,
                    table.Key.TableName,
                    table.Select(x => new ColumnInformation(
                        x.ColumnName,
                        x.DataType,
                        x.IsPrimaryKey,
                        x.IsIdentity,
                        x.IsNullable,
                        x.IsComputed,
                        x.DefaultValue,
                        x.MaxLength,
                        x.Precision,
                        x.Scale))));

            var schema = new SchemaInformation(tables);

            foreach (var group in foreignKeys.GroupBy(x => (x.ConstraintName, x.SchemaName, x.TableName, x.ReferencedSchemaName, x.ReferencedTableName)))
            {
                var constraintName = group.Key.ConstraintName;
                var table = schema[$"{group.Key.SchemaName}.{group.Key.TableName}"];
                var referenceTable = schema[$"{group.Key.ReferencedSchemaName}.{group.Key.ReferencedTableName}"];

                if (table is null || referenceTable is null)
                {
                    throw new SchemaInformationException($"Could not resolve tables for foreign key {constraintName}");
                }

                var fkColumns = GetForeignKeyColumns(constraintName, table, referenceTable, group);
                table.AddForeignKey(new ForeignKeyInformation(group.Key.ConstraintName, referenceTable, fkColumns));
            }

            foreach (var group in triggers.GroupBy(x => (x.SchemaName, x.TableName)))
            {
                var table = schema[$"{group.Key.SchemaName}.{group.Key.TableName}"];
                if (table is null)
                {
                    throw new SchemaInformationException($"Could not resolve tables for triggers {string.Join(", ", group.Select(x => x.Name))}");
                }

                foreach (var trigger in group)
                {
                    table.AddTrigger(new TriggerInformation(trigger.Name, trigger.IsDisabled));
                }
            }

            return schema;
        }

        private IEnumerable<(ColumnInformation, ColumnInformation)> GetForeignKeyColumns(string constraintName, TableInformation table, TableInformation referenceTable, IEnumerable<ForeignKey> fkColumns)
        {
            foreach (var fk in fkColumns)
            {
                if (table[fk.ColumnName] is { } column && referenceTable[fk.ReferencedColumnName] is { } referenceColumn)
                {
                    yield return (column, referenceColumn);
                }
                else
                {
                    throw new SchemaInformationException($"Could not resolve columns for foreign key {constraintName}");
                }
            }
        }

        private async Task<(IEnumerable<SysColumns>, IEnumerable<ForeignKey>, IEnumerable<Trigger> triggers)> LoadSchema(IDbConnection connection, IDbTransaction? transaction = null)
        {
            using var reader = await connection.QueryMultipleAsync(
                @"SELECT 
                    s.name as TableSchema,
                    t.name as TableName,
                    c.name as ColumnName,
                    y.name as DataType,
	                ISNULL(i.is_primary_key, 0) AS IsPrimaryKey,
	                c.is_nullable As IsNullable,
	                c.is_identity as IsIdentity,
                    c.is_computed as IsComputed,
                    OBJECT_DEFINITION(c.default_object_id) as DefaultValue,
                    c.max_length AS MaxLength,
                    c.precision as Precision,
                    c.Scale as Scale
                FROM sys.tables t
                INNER JOIN sys.columns c on c.object_id = t.object_id
                INNER JOIN sys.types y ON c.user_type_id = y.user_type_id
                INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
                LEFT JOIN sys.index_columns ic ON c.column_id = ic.column_id AND ic.object_id = c.object_id
                LEFT JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id AND i.is_primary_key = 1
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
                INNER JOIN sys.tables referenced_table ON fk.referenced_object_id = referenced_table.object_id

                SELECT 
	                SCHEMA_NAME (t.schema_id) AS SchemaName,
	                OBJECT_NAME (parent_id) as TableName,
	                tr.name AS Name,
	                is_disabled AS IsDisabled
                FROM sys.triggers tr
                INNER JOIN sys.tables t ON tr.parent_id= t.object_id",
                transaction: transaction);

            var allColumns = await reader.ReadAsync<SysColumns>();
            var allForeignKeys = await reader.ReadAsync<ForeignKey>();
            var triggers = await reader.ReadAsync<Trigger>();
            return (allColumns, allForeignKeys, triggers);
        }

        private class SysColumns
        {
            public string TableSchema { get; set; } = default!;
            public string TableName { get; set; } = default!;
            public string ColumnName { get; set; } = default!;
            public string DataType { get; set; } = default!;
            public bool IsPrimaryKey { get; set; }
            public bool IsIdentity { get; set; }
            public bool IsNullable { get; set; }
            public bool IsComputed { get; set; }
            public string? DefaultValue { get; set; }
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

        private class Trigger
        {
            public string SchemaName { get; set; } = default!;
            public string TableName { get; set; } = default!;
            public string Name { get; set; } = default!;
            public bool IsDisabled { get; set; }
        }
    }
}
