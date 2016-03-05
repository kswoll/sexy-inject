using System;
using System.Collections.Generic;

namespace SexyInject
{
    public class ConcurrentCache : ICache
    {
        private object lockObject = new object();
        private Dictionary<Type, object> storage = new Dictionary<Type, object>();

        public object Get(Type type)
        {
            lock (lockObject)
            {
                object result;
                storage.TryGetValue(type, out result);
                return result;
            }
        }

        public void Set(Type type, object value)
        {
            lock (lockObject)
            {
                storage[type] = value;                
            }
        }         
    }
}