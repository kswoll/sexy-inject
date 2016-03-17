using System;
using System.Collections.Concurrent;

namespace SexyInject
{
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