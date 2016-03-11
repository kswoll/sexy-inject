using System;
using System.Collections.Concurrent;

namespace SexyInject
{
    public class CacheResolver : IResolver
    {
        private readonly IResolver resolver;
        private readonly Func<ResolveContext, Type, object> keySelector;
        private readonly ConcurrentDictionary<object, Tuple<object, bool>> cache = new ConcurrentDictionary<object, Tuple<object, bool>>();

        public CacheResolver(IResolver resolver, Func<ResolveContext, Type, object> keySelector)
        {
            this.resolver = resolver;
            this.keySelector = keySelector;
        }

        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            var cachedResult = cache.GetOrAdd(keySelector(context, targetType), _ =>
            {
                object innerResult;
                var found = resolver.TryResolve(context, targetType, out innerResult);
                return Tuple.Create(innerResult, found);
            });
            result = cachedResult.Item1;
            return cachedResult.Item2;
        }
    }
}