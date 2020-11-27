using System;
using System.Collections.Generic;
using System.Linq;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.Insertion
{
    public class UniqueValueGenerator
    {
        private static int _currentInteger = 1;
        public UniqueValueGenerator()
        {
            Maps = new ()
            {
                ["uniqueidentifier"] = () => Guid.NewGuid(),
                ["nvarchar"] = () => Guid.NewGuid().ToString(),
                ["varchar"] = () => Guid.NewGuid().ToString(),
                ["shortint"] = () => _currentInteger++,
                ["int"] = () => _currentInteger++,
                ["bigint"] = () => _currentInteger++,
            };
        }

        protected Dictionary<string, Func<object>> Maps { get; }

        public virtual bool CanHandle(ColumnInformation column) => Maps.ContainsKey(column.DataType);
        public virtual object GenerateValue(InsertContext context, ColumnInformation column)
        {
            // Attempt to generate new id's based on previously inserted rows or mapping
            return context.InsertedRows.Where(x => x.Table == column.Table).LastOrDefault()?[column.Name] switch
            {
                int v => v + 1,
                long l => l + 1,
                short s => s + 1,
                _ => Maps[column.DataType]()
            };
        }
    }
}
