using System;

namespace SexyInject
{
    /// <summary>
    /// Used to add arbitrary behavior around an object after it has been resolved.
    /// </summary>
    public class WhenResolvedResolver : IResolver
    {
        private readonly IResolver resolver;
        private readonly Action<ResolveContext, object> handler;

        public WhenResolvedResolver(IResolver resolver, Action<ResolveContext, object> handler)
        {
            this.resolver = resolver;
            this.handler = handler;
        }

        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            if (resolver.TryResolve(context, targetType, out result))
            {
                handler(context, result);
                return true;
            }
            return false;
        }
    }
}