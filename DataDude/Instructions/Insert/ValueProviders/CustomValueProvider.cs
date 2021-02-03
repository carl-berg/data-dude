using System;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    /// <summary>
    /// Provides an extension point where custom values can be configured.
    /// </summary>
    public class CustomValueProvider : IValueProvider
    {
        private readonly Action<ColumnInformation, ColumnValue> _action;

        public CustomValueProvider(Action<ColumnInformation, ColumnValue> action)
        {
            _action = action;
        }

        public void Process(ColumnInformation column, ColumnValue previousValue)
        {
            _action(column, previousValue);
        }
    }
}
