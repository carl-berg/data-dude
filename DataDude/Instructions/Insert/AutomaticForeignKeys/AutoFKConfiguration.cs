namespace DataDude.Instructions.Insert.AutomaticForeignKeys
{
    public class AutoFKConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether "missing" foreign key dependencies will automatically be added as insert instructions.
        /// </summary>
        public bool AddMissingForeignKeys { get; set; }
    }
}
