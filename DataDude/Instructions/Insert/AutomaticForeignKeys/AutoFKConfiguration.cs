using DataDude.Schema;

namespace DataDude.Instructions.Insert.AutomaticForeignKeys
{
    public class AutoFKConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether "missing" foreign key dependencies will automatically be added as insert instructions.
        /// </summary>
        public bool AddMissingForeignKeys { get; set; }

        /// <summary>
        /// Gets or sets a traversal strategy for dependencies. Default value is to follow all foreign keys.
        /// </summary>
        public IDependencyTraversalStrategy DependencyTraversalStrategy { get; set; } = Schema.DependencyTraversalStrategy.FollowAllForeignKeys;
    }
}
