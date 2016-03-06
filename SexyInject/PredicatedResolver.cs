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

        public object Resolve(ResolverContext context, out bool isResolved)
        {
            if (predicate(context))
            {
                return resolver.Resolve(context, out isResolved);
            }
            isResolved = false;
            return null;
        }
    }
}