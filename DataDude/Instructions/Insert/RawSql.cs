namespace DataDude.Instructions.Insert
{
    public struct RawSql
    {
        private string _value;
        public RawSql(string value) => _value = value;
        public override string ToString() => _value;
    }
}
