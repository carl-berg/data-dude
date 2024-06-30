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

        /// <summary>
        /// Enables automatic handling of foreign keys.
        /// </summary>
        /// <remarks>
        /// This means that the key from an insert into a table can be used to fill foreign key constraints in subsequent 
        /// </remarks>
        public static Dude EnableAutomaticForeignKeys(this Dude dude) => dude
            .DisableAutomaticForeignKeys()
            .ConfigureInsert(insertContext =>
            {
                // Insert first in order to run before identity insert interceptor
                insertContext.InsertInterceptors.Insert(0, new ForeignKeyInterceptor());
            });

        /// <summary>
        /// Enables automatic handling of foreign keys and also automatic inserts of missing required (according to the supplied dependency traversal strategy) dependencies.
        /// Default dependency traversal strategy is <see cref="DependencyTraversalStrategy.SkipNullableFKTraversalStrategy"/>.
        /// </summary>
        /// <remarks>
        /// This is the same configuring <see cref="EnableAutomaticForeignKeys"/>, but will also attempt to add missing insert statements
        /// </remarks>
        public static Dude EnableAutomaticInsertOfForeignKeys(this Dude dude, IDependencyTraversalStrategy? dependencyTraversalStrategy = null) => dude
            .EnableAutomaticForeignKeys()
            .Configure(ctx =>
            {
                var dependencyService = new DependencyService(dependencyTraversalStrategy ?? DependencyTraversalStrategy.SkipNullableForeignKeys, ctx);
                ctx.InstructionDecorators.Add(new AddMissingInsertInstructionsPreProcessor(dependencyService));
            });

        /// <summary>
        /// Disables automatic handling of foreign keys (including automatic inserts)
        /// </summary>
        public static Dude DisableAutomaticForeignKeys(this Dude dude) => dude
            .DisableAutomaticInsertOfForeignKeys()
            .ConfigureInsert(insertContext =>
            {
                // Clean out existing ForeignKeyInterceptor
                var existingInterceptors = insertContext.InsertInterceptors.OfType<ForeignKeyInterceptor>().ToList();
                foreach (var existingFKInterceptors in existingInterceptors)
                {
                    insertContext.InsertInterceptors.Remove(existingFKInterceptors);
                }
            });

        /// <summary>
        /// Disables automatic inserts of foreign keys (but not automatic handling of foreign keys)
        /// </summary>
        public static Dude DisableAutomaticInsertOfForeignKeys(this Dude dude) => dude
            .Configure(x =>
            {
                // Clean out existing AddMissingInsertInstructionsPreProcessors
                var existingPreProcessors = x.InstructionDecorators.OfType<AddMissingInsertInstructionsPreProcessor>().ToList();
                foreach (var existingPreProcessor in existingPreProcessors)
                {
                    x.InstructionDecorators.Remove(existingPreProcessor);
                }
            });

        public static Dude ConfigureCustomColumnValue(this Dude dude, Action<ColumnInformation, ColumnValue> getValue)
        {
            dude.ConfigureInsert(x => x.InsertValueProviders.Insert(0, new CustomValueProvider(getValue)));
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
