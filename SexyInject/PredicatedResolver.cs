using System;

namespace SexyInject
{
    public class PredicatedResolver<T> : IResolver<T>
    {
        private readonly Func<ResolverContext, bool> predicate;
        private readonly IResolver<T> resolver;

        public PredicatedResolver(Func<ResolverContext, bool> predicate, IResolver<T> resolver)
        {
            this.predicate = predicate;
            this.resolver = resolver;
        }

        public T Resolve(ResolverContext context, out bool isResolved)
        {
            if (predicate(context))
            {
                return resolver.Resolve(context, out isResolved);
            }
            isResolved = false;
            return default(T);
        }
    }
}