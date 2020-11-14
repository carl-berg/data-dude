using System.Collections.Generic;

namespace DataDude.Instructions.Insert
{
    public class InsertInstruction : IInstruction
    {
        public InsertInstruction(string tableName, object? columnData = null)
        {
            TableName = tableName;
            ColumnValues = new Dictionary<string, object>();
            foreach (var prop in columnData?.GetType().GetProperties() ?? new System.Reflection.PropertyInfo[0])
            {
                ColumnValues[prop.Name] = prop.GetValue(columnData);
            }
        }

        public string TableName { get; }
        public IDictionary<string, object> ColumnValues { get; }
    }
}
