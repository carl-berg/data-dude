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
        public static Dude Insert(this Dude dude, string table, params object[] rowData)
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

        public static Dude EnableAutomaticForeignKeys(this Dude dude, Action<AutoFKConfiguration>? configure = null)
        {
            var config = new AutoFKConfiguration();
            configure?.Invoke(config);

            // Insert first in order to run before identity insert interceptor
            dude.ConfigureInsert(x => x.InsertInterceptors.Insert(0, new ForeignKeyInterceptor()));

            if (config.AddMissingForeignKeys)
            {
                var dependencyService = new DependencyService(config.DependencyTraversalStrategy);
                dude.Configure(x => x.InstructionPreProcessors.Add(new AddMissingInsertInstructionsPreProcessor(dependencyService)));
            }

            return dude;
        }

        public static Dude ConfigureCustomColumnValue(this Dude dude, Func<ColumnInformation, ColumnValue, bool> match, Func<object> getValue)
        {
            dude.ConfigureInsert(x => x.InsertValueProviders.Insert(0, new CustomValueProvider(match, getValue)));
            return dude;
        }

        public static Dude ConfigureInsert(this Dude dude, Action<InsertContext> configure)
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
