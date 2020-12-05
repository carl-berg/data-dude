using System;
using System.Linq;
using DataDude.Instructions.Insert;
using DataDude.Instructions.Insert.AutomaticForeignKeys;
using DataDude.Instructions.Insert.ValueProviders;
using DataDude.Schema;

namespace DataDude
{
    public static class InsertExtensions
    {
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

        public static DataDude EnableAutomaticForeignKeys(this DataDude dude, Action<AutoFKConfiguration>? configure = null)
        {
            var config = new AutoFKConfiguration();
            configure?.Invoke(config);

            // Insert first in order to run before identity insert interceptor
            dude.ConfigureInsert(x => x.InsertInterceptors.Insert(0, new ForeignKeyInterceptor()));

            if (config.AddMissingForeignKeys)
            {
                dude.Configure(x => x.InstructionPreProcessors.Add(new AddMissingInsertInstructionsPreProcessor()));
            }

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
