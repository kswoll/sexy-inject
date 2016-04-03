using System;

namespace SexyInject.Emit
{
    public class InvalidFactoryException : Exception
    {
        public InvalidFactoryException()
        {
        }

        public InvalidFactoryException(string message) : base(message)
        {
        }

        public InvalidFactoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}