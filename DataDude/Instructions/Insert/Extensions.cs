using System;
using System.Linq;
using DataDude.Instructions.Insert.AutomaticForeignKeys;
using DataDude.Instructions.Insert.Interception;
using DataDude.Instructions.Insert.ValueProviders;
using DataDude.Schema;

namespace DataDude.Instructions.Insert
{
    public static class Extensions
    {
        public static DataDude Insert(this DataDude dude, string table, object? data = null)
        {
            dude.Configure(x => x.Instructions.Add(new InsertInstruction(table, data)));
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
