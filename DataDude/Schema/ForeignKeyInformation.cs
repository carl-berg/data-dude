using System.Collections.Generic;

namespace DataDude.Schema
{
    public class ForeignKeyInformation
    {
        public ForeignKeyInformation(string name, TableInformation referencedTable, IEnumerable<(ColumnInformation Column, ColumnInformation ReferencedColumn)> columns)
        {
            Name = name;
            ReferencedTable = referencedTable;
            Columns = columns;
        }

        public string Name { get; }
        public TableInformation ReferencedTable { get; }
        public IEnumerable<(ColumnInformation Column, ColumnInformation ReferencedColumn)> Columns { get; }
    }
}
