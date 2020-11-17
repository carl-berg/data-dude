using System;

namespace DataDude.Instructions.Insert
{
    public class InsertHandlerException : HandlerException
    {
        public InsertHandlerException(string message, Exception? innerException = null, InsertStatement? statement = null)
            : base(message, innerException)
        {
            Statement = statement;
        }

        public InsertStatement? Statement { get; }
    }
}
