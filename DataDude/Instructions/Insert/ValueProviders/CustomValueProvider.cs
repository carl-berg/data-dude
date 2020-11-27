using System;
using System.Collections.Generic;
using System.Linq;
using DataDude.Schema;

namespace DataDude.Instructions.Insert.ValueProviders
{
    /// <summary>
    /// Provides an extension point where custom values can be configured.
    /// </summary>
    public class CustomValueProvider : IValueProvider
    {
        private readonly IEnumerable<DefaultValue> _defaultValues;

        public CustomValueProvider(IEnumerable<DefaultValue> defaultValues)
        {
            _defaultValues = defaultValues;
        }

        public void Process(ColumnInformation column, ColumnValue previousValue)
        {
            foreach (var df in _defaultValues.Where(x => x.Match(column, previousValue)))
            {
                previousValue.Set(new ColumnValue(df.Value));
            }
        }

        public class DefaultValue
        {
            public DefaultValue(Func<ColumnInformation, ColumnValue, bool> match, object value)
            {
                Match = match;
                Value = value;
            }

            public Func<ColumnInformation, ColumnValue, bool> Match { get; }
            public object Value { get; }
        }
    }
}
