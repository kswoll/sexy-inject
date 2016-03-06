using System;

namespace SexyInject
{
    public class PredicatedResolver : IResolver
    {
        private readonly IResolver resolver;
        private readonly Func<ResolveContext, Type, bool> predicate;

        public PredicatedResolver(IResolver resolver, Func<ResolveContext, Type, bool> predicate)
        {
            this.resolver = resolver;
            this.predicate = predicate;
        }

        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            if (predicate(context, targetType))
            {
                return resolver.TryResolve(context, targetType, out result);
            }
            result = null;
            return false;
        }
    }
}