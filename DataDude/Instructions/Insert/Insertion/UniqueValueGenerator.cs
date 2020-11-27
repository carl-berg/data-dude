using System;
using System.Collections.Generic;
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
        public virtual object GenerateValue(ColumnInformation column) => Maps[column.DataType]();
    }
}
