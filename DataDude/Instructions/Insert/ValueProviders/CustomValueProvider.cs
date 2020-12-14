using System;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    /// <summary>
    /// Provides an extension point where custom values can be configured.
    /// </summary>
    public class CustomValueProvider : IValueProvider
    {
        private readonly Func<ColumnInformation, ColumnValue, bool> _match;
        private readonly Func<object> _getValue;

        public CustomValueProvider(Func<ColumnInformation, ColumnValue, bool> match, Func<object> getValue)
        {
            _match = match;
            _getValue = getValue;
        }

        public void Process(ColumnInformation column, ColumnValue previousValue)
        {
            if (_match(column, previousValue))
            {
                previousValue.Set(new ColumnValue(_getValue()));
            }
        }
    }
}
