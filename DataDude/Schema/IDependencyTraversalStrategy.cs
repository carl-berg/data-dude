namespace DataDude.Schema
{
    public interface IDependencyTraversalStrategy
    {
        bool Process(ForeignKeyInformation foreignKey);
    }
}
