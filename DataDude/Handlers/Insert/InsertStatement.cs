using System.Collections.Generic;
using DataDude.Instructions;
using DataDude.Schema;

namespace DataDude.Handlers.Insert
{
    public class InsertStatement : Dictionary<ColumnInformation, ColumnValue>
    {
        public InsertStatement(TableInformation table, InsertInstruction instruction)
        {
            Table = table;
            foreach (var column in table)
            {
                if (instruction.ColumnValues.TryGetValue(column.Name, out var value) && value is { })
                {
                    Add(column, new ColumnValue(value));
                }
                else
                {
                    Add(column, ColumnValue.NotSet());
                }
            }
        }

        public TableInformation Table { get; }

        public void InvokeValueHandler(IDataDudeInsertValueHandler handler)
        {
            foreach (var column in this)
            {
                this[column.Key].Set(handler.Handle(Table, column.Key, column.Value));
            }
        }
    }
}
