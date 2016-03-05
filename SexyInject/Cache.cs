using System;
using System.Collections.Generic;

namespace SexyInject
{
    public class Cache : ICache
    {
        private Dictionary<Type, object> storage = new Dictionary<Type, object>();

        public object Get(Type type)
        {
            object result;
            storage.TryGetValue(type, out result);
            return result;
        }

        public void Set(Type type, object value)
        {
            storage[type] = value;
        }
    }
}