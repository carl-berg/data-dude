﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDude.Schema;

namespace DataDude.Instructions.Insert
{
    public class InsertInstructionHandler : IInstructionHandler
    {
        public virtual async Task<HandleInstructionResult> Handle(IInstruction instruction, DataDudeContext context, IDbConnection connection, IDbTransaction? transaction = null)
        {
            if (instruction is InsertInstruction insert)
            {
                if (context.Schema?[insert.TableName] is { } table)
                {
                    var statement = new InsertStatement(table, insert);

                    try
                    {
                        foreach (var valueHandler in context.InsertValueProviders)
                        {
                            statement.InvokeValueProvider(valueHandler);
                        }

                        foreach (var insertInterceptor in context.InsertInterceptors)
                        {
                            await insertInterceptor.OnInsert(statement, context, connection, transaction);
                        }

                        var insertedRow = await Insert(statement, connection, transaction);

                        foreach (var insertInterceptor in context.InsertInterceptors)
                        {
                            await insertInterceptor.OnInserted(insertedRow, statement, context, connection, transaction);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new HandlerException($"Insertion into table {insert.TableName} failed", ex);
                    }
                }
                else
                {
                    throw new HandlerException($"Could not find table {insert.TableName} in schema");
                }

                return new HandleInstructionResult(true);
            }

            return new HandleInstructionResult(false);
        }

        protected virtual async Task<IDictionary<string, object>> Insert(InsertStatement statement, IDbConnection connection, IDbTransaction? transaction = null)
        {
            var columnsToInsert = statement.Data.Where(x => x.Value.Type == ColumnValueType.Set);
            var columns = string.Join(", ", columnsToInsert.Select(x => x.Column.Name));
            var values = string.Join(", ", columnsToInsert.Select(x => GetParameterNameOrRawSql(x.Column, x.Value)));
            var parameters = new DynamicParameters();
            foreach (var (column, value) in columnsToInsert)
            {
                if (!(value.Value is RawSql))
                {
                    parameters.Add(column.Name, value.Value, value.DbType);
                }
            }

            // How do we best handle fetching inserted row? This won't work if there are triggers on the table, they will need to be disabled during this execution
            // which is feasible, but maybe not expected?
            var insertedRow = await connection.QuerySingleAsync<dynamic>(
                $@"INSERT INTO {statement.Table.Schema}.{statement.Table.Name}({columns}) OUTPUT inserted.* VALUES({values})",
                parameters,
                transaction);

            if (insertedRow is IDictionary<string, object> { } typedRow)
            {
                return typedRow;
            }
            else
            {
                throw new HandlerException("Could not parse inserted row as dictionary");
            }
        }

        private string GetParameterNameOrRawSql(ColumnInformation column, ColumnValue columnValue)
        {
            if (columnValue.Value is RawSql rawSql)
            {
                return rawSql.ToString();
            }

            return $"@{column.Name}";
        }
    }
}
