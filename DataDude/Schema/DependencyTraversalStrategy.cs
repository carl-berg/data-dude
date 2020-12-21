using System.Linq;

namespace DataDude.Schema
{
    public static class DependencyTraversalStrategy
    {
        /// <summary>
        /// Gets a strategy that does not follow nullable dependencies.
        /// </summary>
        public static IDependencyTraversalStrategy SkipNullableForeignKeys => new SkipNullableFKTraversalStrategy();

        /// <summary>
        /// Gets a strategy that does not follow recursivce dependencies.
        /// </summary>
        public static IDependencyTraversalStrategy SkipRecursiveForeignKeys => new SkipRecursiveFKTraversalStrategy();

        /// <summary>
        /// Gets a strategy that follows all dependencies.
        /// </summary>
        public static IDependencyTraversalStrategy FollowAllForeignKeys => new FollowAllForeignKeysTraversalStrategy();

        private class SkipNullableFKTraversalStrategy : IDependencyTraversalStrategy
        {
            public bool Process(ForeignKeyInformation foreignKey)
            {
                return foreignKey.Columns.All(c => c.Column.IsNullable == false);
            }
        }

        private class SkipRecursiveFKTraversalStrategy : IDependencyTraversalStrategy
        {
            public bool Process(ForeignKeyInformation foreignKey)
            {
                return foreignKey.Table != foreignKey.ReferencedTable;
            }
        }

        private class FollowAllForeignKeysTraversalStrategy : IDependencyTraversalStrategy
        {
            public bool Process(ForeignKeyInformation foreignKey) => true;
        }
    }
}
