using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DataDude.Schema;

namespace DataDude.SqlServer
{
    public class SqlServerSchemaLoader : ISchemaLoader
    {
        public bool CacheSchema { get; set; } = true;

        public async Task<SchemaInformation> Load(DbConnection connection, DbTransaction? transaction = null)
        {
            var (columns, indexes, foreignKeys, triggers) = await LoadSchema(connection, transaction);

            var tables = columns
                .GroupBy(x => (x.TableSchema, x.TableName))
                .Select(table => new TableInformation(
                    table.Key.TableSchema,
                    table.Key.TableName,
                    t => table.Select(x => new ColumnInformation(
                        t,
                        x.ColumnName,
                        x.DataType,
                        x.IsIdentity,
                        x.IsNullable,
                        x.IsComputed,
                        x.DefaultValue,
                        x.MaxLength,
                        x.Precision,
                        x.Scale))));

            var schema = new SchemaInformation(tables);

            foreach (var group in indexes.GroupBy(x => (x.Name, x.TableSchema, x.TableName, x.IsPrimaryKey, x.IsUnique, x.IsUniqueConstraint, x.IsDisabled)))
            {
                var indexName = group.Key.Name;
                var table = schema[$"{group.Key.TableSchema}.{group.Key.TableName}"];

                if (table is null)
                {
                    throw new SchemaInformationException($"Could not resolve table for index {indexName}");
                }

                var indexColumns = group.Select(x => table[x.ColumnName]);
                if (indexColumns?.Any(x => x == null) ?? true)
                {
                    throw new SchemaInformationException($"Could not resolve all columns for index {indexName}");
                }

                table.AddIndex(new IndexInformation(
                    table,
                    indexName,
                    indexColumns!,
                    group.Key.IsPrimaryKey,
                    group.Key.IsUnique,
                    group.Key.IsUniqueConstraint,
                    group.Key.IsDisabled));
            }

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
                table.AddForeignKey(new ForeignKeyInformation(group.Key.ConstraintName, table, referenceTable, fkColumns));
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
                    table.AddTrigger(new TriggerInformation(table, trigger.Name, trigger.IsDisabled));
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

        private async Task<(IEnumerable<SysColumns>, IEnumerable<Indexes>, IEnumerable<ForeignKey>, IEnumerable<Trigger> triggers)> LoadSchema(DbConnection connection, DbTransaction? transaction = null)
        {
            var command = connection.CreateCommand();

            if (command is null)
            {
                throw new Exception();
            }

            command.Transaction = transaction;
            command.CommandText = @"SELECT 
                    s.name as TableSchema,
                    t.name as TableName,
                    c.name as ColumnName,
                    y.name as DataType,
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
                ORDER BY s.name, t.name

                SELECT 
	                i.name as Name, 
	                s.name as TableSchema,
	                OBJECT_NAME(i.object_id) AS TableName, 
	                COL_NAME(i.object_id, ic.column_id) AS ColumnName,
	                i.is_primary_key AS IsPrimaryKey,
	                i.is_unique AS IsUnique,
	                i.is_unique_constraint AS IsUniqueConstraint,
                    i.is_disabled AS IsDisabled
                FROM sys.index_columns ic
                INNER JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN sys.tables t on i.object_id = t.object_id
                INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
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
                INNER JOIN sys.tables t ON tr.parent_id= t.object_id";

            using var reader = command.ExecuteReader(CommandBehavior.Default);
            var allColumns = await ReadColumns(reader);
            await reader.NextResultAsync();
            var allIndexes = await ReadIndexes(reader);
            await reader.NextResultAsync();
            var allForeignKeys = await ReadForeignKeys(reader);
            await reader.NextResultAsync();
            var triggers = await ReadTriggers(reader);
            reader.Close();
            return (allColumns, allIndexes, allForeignKeys, triggers);
        }

        private async Task<IReadOnlyList<SysColumns>> ReadColumns(DbDataReader reader)
        {
            var items = new List<SysColumns>();
            while(await reader.ReadAsync())
            {
                items.Add(new SysColumns
                {
                    TableSchema = reader.GetString(0),
                    TableName = reader.GetString(1),
                    ColumnName = reader.GetString(2),
                    DataType = reader.GetString(3),
                    IsNullable = reader.GetBoolean(4),
                    IsIdentity = reader.GetBoolean(5),
                    IsComputed = reader.GetBoolean(6),
                    DefaultValue = reader.IsDBNull(7) ? null : reader.GetString(7),
                    MaxLength = Convert.ToInt32(reader.GetValue(8)),
                    Precision = Convert.ToInt32(reader.GetValue(9)),
                    Scale = Convert.ToInt32(reader.GetValue(10)),
                });
            }

            return items;
        }

        private async Task<IReadOnlyList<Indexes>> ReadIndexes(DbDataReader reader)
        {
            var items = new List<Indexes>();
            while (await reader.ReadAsync())
            {
                items.Add(new Indexes
                {
                    Name = reader.GetString(0),
                    TableSchema = reader.GetString(1),
                    TableName = reader.GetString(2),
                    ColumnName = reader.GetString(3),
                    IsPrimaryKey = reader.GetBoolean(4),
                    IsUnique = reader.GetBoolean(5),
                    IsUniqueConstraint = reader.GetBoolean(6),
                    IsDisabled = reader.GetBoolean(7),
                });
            }

            return items;
        }

        private async Task<IReadOnlyList<ForeignKey>> ReadForeignKeys(DbDataReader reader)
        {
            var items = new List<ForeignKey>();
            while (await reader.ReadAsync())
            {
                items.Add(new ForeignKey
                {
                    ConstraintName = reader.GetString(0),
                    SchemaName = reader.GetString(1),
                    TableName = reader.GetString(2),
                    ColumnName = reader.GetString(3),
                    ReferencedSchemaName = reader.GetString(4),
                    ReferencedTableName = reader.GetString(5),
                    ReferencedColumnName = reader.GetString(6),
                });
            }

            return items;
        }

        private async Task<IReadOnlyList<Trigger>> ReadTriggers(DbDataReader reader)
        {
            var items = new List<Trigger>();
            while (await reader.ReadAsync())
            {
                items.Add(new Trigger
                { 
                    SchemaName = reader.GetString(0),
                    TableName = reader.GetString(1),
                    Name = reader.GetString(2),
                    IsDisabled = reader.GetBoolean(3),
                });
            }

            return items;
        }

        private class SysColumns
        {
            public string TableSchema { get; set; } = default!;
            public string TableName { get; set; } = default!;
            public string ColumnName { get; set; } = default!;
            public string DataType { get; set; } = default!;
            public bool IsIdentity { get; set; }
            public bool IsNullable { get; set; }
            public bool IsComputed { get; set; }
            public string? DefaultValue { get; set; }
            public int MaxLength { get; set; }
            public int Precision { get; set; }
            public int Scale { get; set; }
        }

        private class Indexes
        {
            public string Name { get; set; } = default!;
            public string TableSchema { get; set; } = default!;
            public string TableName { get; set; } = default!;
            public string ColumnName { get; set; } = default!;
            public bool IsPrimaryKey { get; set; }
            public bool IsUnique { get; set; }
            public bool IsUniqueConstraint { get; set; }
            public bool IsDisabled { get; set; }
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
