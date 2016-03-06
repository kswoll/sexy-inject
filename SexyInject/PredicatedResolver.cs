using System;

namespace SexyInject
{
    public class PredicatedResolver : IResolver
    {
        private readonly Func<ResolverContext, bool> predicate;
        private readonly IResolver resolver;

        public PredicatedResolver(Func<ResolverContext, bool> predicate, IResolver resolver)
        {
            this.predicate = predicate;
            this.resolver = resolver;
        }

        public bool TryResolve(ResolverContext context, out object result)
        {
            if (predicate(context))
            {
                return resolver.TryResolve(context, out result);
            }
            result = null;
            return false;
        }
    }
}