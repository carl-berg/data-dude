using DataDude.Instructions;

namespace DataDude
{
    public static class Extensions
    {
        /// <summary>Enables static caching.</summary>
        /// <remarks>Note that static caching is enabled by default, so this is only needed to re-enable it after it has been disabled.</remarks>
        public static Dude EnableStaticCaching(this Dude dude) => dude.Configure(context =>
        {
            DisableStaticCaching(dude);
            context.InstructionDecorators.Add(new StaticCache());
        });

        /// <summary>Disables static caching.</summary>
        public static Dude DisableStaticCaching(this Dude dude) => dude.Configure(context =>
        {
            var staticCaches = context.InstructionDecorators.OfType<StaticCache>().ToList();
            foreach (var cacheDecorator in staticCaches)
            {
                context.InstructionDecorators.Remove(cacheDecorator);
            }
        });
    }
}
