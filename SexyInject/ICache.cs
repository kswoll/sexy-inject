using System;

namespace SexyInject
{
    public interface ICache
    {
        object Get(Type type); 
        void Set(Type type, object value);
    }
}