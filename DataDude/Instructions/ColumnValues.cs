using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataDude.Instructions
{
    public class ColumnValues : Dictionary<string, object>
    {
        public ColumnValues(object? data)
        {
            if (data is IDictionary<string, object> columnValues)
            {
                foreach(var columnValue in columnValues)
                {
                    Add(columnValue.Key, columnValue.Value);
                }
            }
            else
            {
                foreach (var prop in data?.GetType().GetProperties() ?? Enumerable.Empty<PropertyInfo>())
                {
                    this[prop.Name] = prop.GetValue(data);
                }
            }
        }
    }
}
