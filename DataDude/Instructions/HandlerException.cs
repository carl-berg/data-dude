using System;

namespace DataDude.Instructions
{
    public class HandlerException : Exception
    {
        public HandlerException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
