﻿using System.Collections;
using System.Collections.Generic;
using DataDude.Instructions.Insert.Insertion;
using DataDude.Schema;

namespace DataDude.Instructions.Insert
{
    public sealed class InsertedRow : IEnumerable<(string Column, object Value)>
    {
        private readonly IReadOnlyDictionary<string, object> _insertedRow;
        public InsertedRow(TableInformation table, IReadOnlyDictionary<string, object> insertedRow, IInsertRowHandler handler)
        {
            Table = table;
            Handler = handler;
            _insertedRow = insertedRow;
        }

        public TableInformation Table { get; }
        public IInsertRowHandler Handler { get; }
        public int Count => _insertedRow.Count;
        public object this[string key] => _insertedRow[key];
        public bool ContainsKey(string key) => _insertedRow.ContainsKey(key);

        public IEnumerator<(string Column, object Value)> GetEnumerator()
        {
            foreach (var item in _insertedRow)
            {
                yield return (item.Key, item.Value);
            }
        }

        public bool TryGetValue(string key, out object value) => _insertedRow.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
