using System;

namespace SexyInject
{
    /// <summary>
    /// Used to add arbitrary behavior around an object after it has been resolved.
    /// </summary>
    public class WhenResolvedResolver : IResolverOperator
    {
        private readonly Action<ResolveContext, object> handler;

        public WhenResolvedResolver(Action<ResolveContext, object> handler)
        {
            this.handler = handler;
        }

        public bool TryResolve(ResolveContext context, Type targetType, ResolverProcessor resolverProcessor, out object result)
        {
            if (resolverProcessor(context, targetType, out result))
            {
                handler(context, result);
                return true;
            }
            return false;
        }
    }
}