using System;

namespace SexyInject
{
    public class TransientCacheResolver : IResolverOperator
    {
        public bool TryResolve(ResolveContext context, Type targetType, ResolverProcessor resolverProcessor, out object result)
        {
            if (!context.TryRetrieveFromCache(targetType, out result))
            {
                var found = resolverProcessor(context, targetType, out result);
                context.Cache(targetType, result);
                if (!found)
                    return false;
            }
            return true;
        }
    }
}