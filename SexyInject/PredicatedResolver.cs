using System;

namespace SexyInject
{
    public class PredicatedResolver : IResolverOperator
    {
        private readonly Func<ResolveContext, Type, bool> predicate;

        public PredicatedResolver(Func<ResolveContext, Type, bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool TryResolve(ResolveContext context, Type targetType, ResolverProcessor resolverProcessor, out object result)
        {
            if (predicate(context, targetType))
            {
                return resolverProcessor(context, targetType, out result);
            }
            result = null;
            return false;
        }
    }
}