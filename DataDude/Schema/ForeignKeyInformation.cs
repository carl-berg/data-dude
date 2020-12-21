using System.Collections.Generic;

namespace DataDude.Schema
{
    public class ForeignKeyInformation
    {
        public ForeignKeyInformation(string name, TableInformation table, TableInformation referencedTable, IEnumerable<(ColumnInformation Column, ColumnInformation ReferencedColumn)> columns)
        {
            Name = name;
            Table = table;
            ReferencedTable = referencedTable;
            Columns = columns;
        }

        public string Name { get; }
        public TableInformation Table { get; }
        public TableInformation ReferencedTable { get; }
        public IEnumerable<(ColumnInformation Column, ColumnInformation ReferencedColumn)> Columns { get; }
    }
}
