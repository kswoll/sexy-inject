using System;
using System.Collections.Generic;
using System.Reflection;

namespace SexyInject
{
    public class ResolverContext
    {
        private Dictionary<Type, object> cache = new Dictionary<Type, object>();
        private Func<Type, object> resolver;

        internal static MethodInfo resolveMethod = typeof(ResolverContext).GetMethod(nameof(Resolve));

        public ResolverContext()
        {
        }

        public object Resolve(Type type)
        {
            return resolver(type);
        }
    }
}