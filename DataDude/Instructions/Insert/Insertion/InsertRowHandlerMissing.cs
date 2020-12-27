namespace DataDude.Instructions.Insert.Insertion
{
    public class InsertRowHandlerMissing : InsertHandlerException
    {
        public InsertRowHandlerMissing(InsertStatement statement)
            : base($"Could not find an insert row handler for insert into table {statement.Table.Name}. Try configuring OutputInsertRowHandler or plug in your own insert row handler", statement: statement)
        {
        }
    }
}
