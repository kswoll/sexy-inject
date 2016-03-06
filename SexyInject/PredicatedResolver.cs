using System;

namespace SexyInject
{
    public class PredicatedResolver : IResolver
    {
        private readonly Func<ResolverContext, Type, bool> predicate;
        private readonly IResolver resolver;

        public PredicatedResolver(Func<ResolverContext, Type, bool> predicate, IResolver resolver)
        {
            this.predicate = predicate;
            this.resolver = resolver;
        }

        public bool TryResolve(ResolverContext context, Type targetType, out object result)
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