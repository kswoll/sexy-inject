using System;

namespace SexyInject
{
    public class RegistryException : Exception
    {
        public Type[] ResolutionPath { get; }

//        public RegistryException()
//        {
//        }

        public RegistryException(Type[] resolutionPath, string message) : base(message)
        {
            ResolutionPath = resolutionPath;
        }

        public RegistryException(Type[] resolutionPath, string message, Exception inner) : base(message, inner)
        {
            ResolutionPath = resolutionPath;
        }


//        public RegistryException(string message, Exception innerException) : base(message, innerException)
//        {
//        }
    }
}