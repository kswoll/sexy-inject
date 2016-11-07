using System;

namespace SexyInject.Emit
{
    public class ConstructorInvocationException : Exception
    {
        public ConstructorInvocationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}