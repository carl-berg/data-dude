using System;
using System.Collections;
using System.Collections.Generic;
using DataDude.Schema;

namespace DataDude.Tests.Core
{
    public class TestTable : TableInformation
    {
        public TestTable(string name)
            : this(name, c => c.AddIntegerColumn("Id", true))
        {
            AddIndex(new IndexInformation(this, $"PK_{name}", new[] { this["Id"] ! }, true, false, false, false));
        }

        public TestTable(string name, Action<TestTableColumnBuilder> configureColumns)
            : base("dbo", name, table =>
            {
                var builder = new TestTableColumnBuilder(table);
                configureColumns(builder);
                return builder;
            })
        {
        }

        public TestTable AddFk(params TableInformation[] to)
        {
            foreach (var referenceTable in to)
            {
                var fk = new ForeignKeyInformation(
                    $"FK_{Name}_{referenceTable.Name}",
                    this,
                    referenceTable,
                    new[] { (this["Id"], referenceTable["Id"]) });
                AddForeignKey(fk);
            }

            return this;
        }

        public class TestTableColumnBuilder : IEnumerable<ColumnInformation>
        {
            private readonly TableInformation _table;
            private readonly List<ColumnInformation> _columns = new List<ColumnInformation>();
            public TestTableColumnBuilder(TableInformation table) => _table = table;
            public TestTableColumnBuilder AddIntegerColumn(string name, bool identity = false)
            {
                _columns.Add(new ColumnInformation(_table, name, "int", identity, false, false, null, 4, 0, 0));
                return this;
            }

            public TestTableColumnBuilder AddStringColumn(string name, int length = 250)
            {
                _columns.Add(new ColumnInformation(_table, name, $"nvarchar", false, false, false, null, length, 0, 0));
                return this;
            }

            public IEnumerator<ColumnInformation> GetEnumerator() => _columns.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
