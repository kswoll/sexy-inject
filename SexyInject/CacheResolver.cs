using System;
using System.Collections.Concurrent;

namespace SexyInject
{
    /// <summary>
    /// Resolver used to implement the Cache operator.  It allows you to provide a key when resolving.  Only one
    /// unique instance per key will be returned.  Think of the key like the key you use in the LINQ .GroupBy 
    /// operator.
    /// </summary>
    public class CacheResolver : IResolverOperator
    {
        private readonly Func<ResolveContext, Type, object> keySelector;
        private readonly ConcurrentDictionary<object, Tuple<object, bool>> cache = new ConcurrentDictionary<object, Tuple<object, bool>>();

        public CacheResolver(Func<ResolveContext, Type, object> keySelector)
        {
            this.keySelector = keySelector;
        }

        public bool TryResolve(ResolveContext context, Type targetType, ResolverProcessor resolverProcessor, out object result)
        {
            var cachedResult = cache.GetOrAdd(keySelector(context, targetType), _ =>
            {
                object innerResult;
                var found = resolverProcessor(context, targetType, out innerResult);
                return Tuple.Create(innerResult, found);
            });
            result = cachedResult.Item1;
            return cachedResult.Item2;
        }
    }
}