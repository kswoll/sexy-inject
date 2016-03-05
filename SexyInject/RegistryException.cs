using System;

namespace SexyInject
{
    public class RegistryException : Exception 
    {
//        public RegistryException()
//        {
//        }

        public RegistryException(string message) : base(message)
        {
        }

//        public RegistryException(string message, Exception innerException) : base(message, innerException)
//        {
//        }
    }
}