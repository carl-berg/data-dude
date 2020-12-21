using System;

namespace DataDude.Schema
{
    public class DependencyTraversalFailedException : Exception
    {
        public DependencyTraversalFailedException(string message)
            : base(message)
        {
        }

        public DependencyTraversalFailedException(TableInformation sourceTable)
            : this($"Failed building a dependency chain for table {sourceTable.FullName}")
        {
        }
    }
}
