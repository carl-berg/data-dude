using System.Collections;
using System.Collections.Generic;
using DataDude.Schema;

namespace DataDude.Instructions.Insert
{
    public class InsertData : IEnumerable<(ColumnInformation Column, ColumnValue Value)>
    {
        private readonly Dictionary<ColumnInformation, ColumnValue> _data;

        public InsertData(TableInformation table, InsertInstruction instruction)
        {
            _data = new Dictionary<ColumnInformation, ColumnValue>();

            foreach (var column in table)
            {
                if (instruction.ColumnValues.TryGetValue(column.Name, out var value) && value is { })
                {
                    _data.Add(column, new ColumnValue(value));
                }
                else
                {
                    _data.Add(column, ColumnValue.NotSet);
                }
            }
        }

        public ColumnValue this[ColumnInformation column] => _data[column];

        public IEnumerator<(ColumnInformation, ColumnValue)> GetEnumerator()
        {
            foreach (var item in _data)
            {
                yield return (item.Key, item.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
