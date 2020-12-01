using System;
using System.Linq;
using DataDude.Instructions.Execute;
using DataDude.Instructions.Insert;
using DataDude.Instructions.Insert.Interception;
using DataDude.Instructions.Insert.ValueProviders;
using DataDude.Schema;

namespace DataDude
{
    public static class DataDudeExtensions
    {
        public static DataDude Execute(this DataDude dude, string sql, object? parameters = null)
        {
            dude.Configure(x => x.Instructions.Add(new ExecuteInstruction(sql, parameters)));
            return dude;
        }

        public static DataDude Insert(this DataDude dude, string table, params object[] rowData)
        {
            if (rowData.Any())
            {
                foreach (var data in rowData)
                {
                    dude.Configure(x => x.Instructions.Add(new InsertInstruction(table, data)));
                }
            }
            else
            {
                dude.Configure(x => x.Instructions.Add(new InsertInstruction(table)));
            }

            return dude;
        }

        public static DataDude EnableAutomaticForeignKeys(this DataDude dude)
        {
            // Insert first in order to run before identity insert interceptor
            dude.ConfigureInsert(x => x.InsertInterceptors.Insert(0, new ForeignKeyInterceptor()));
            return dude;
        }

        public static DataDude ConfigureCustomColumnValues(this DataDude dude, params (Func<ColumnInformation, ColumnValue, bool> Match, object Value)[] customValues)
        {
            var typedCustomValues = customValues.Select(cv => new CustomValueProvider.DefaultValue(cv.Match, cv.Value));
            dude.ConfigureInsert(x => x.InsertValueProviders.Add(new CustomValueProvider(typedCustomValues)));
            return dude;
        }

        public static DataDude ConfigureInsert(this DataDude dude, Action<InsertContext> configure)
        {
            dude.Configure(x =>
            {
                if (InsertContext.Get(x) is { } context)
                {
                    configure(context);
                }
            });
            return dude;
        }
    }
}
