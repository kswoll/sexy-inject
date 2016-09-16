using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SexyInject
{
    /// <summary>
    /// This class is used internally by the registry to keep track of how each type should be resolved.  You
    /// should never need to interact with it directly.
    /// </summary>
    public class Binding
    {
        public Registry Registry { get; }
        public Type Type { get; }

        internal IEnumerable<ResolverContext> ResolverContexts => resolverContexts;

        private ConcurrentQueue<ResolverContext> resolverContexts = new ConcurrentQueue<ResolverContext>();

        internal Binding(Registry registry, Type type)
        {
            Registry = registry;
            Type = type;
        }

        public void AddResolverContext(ResolverContext context)
        {
            resolverContexts.Enqueue(context);
        }

        public void InsertResolverContext(ResolverContext context)
        {
            resolverContexts = new ConcurrentQueue<ResolverContext>(new[] { context }.Concat(resolverContexts));
        }

        public IEnumerable<ResolverProcessor> Resolvers => resolverContexts.Select(x => x.ResolverProcessor);

        public object Resolve(ResolveContext context, Type targetType)
        {
            foreach (var resolver in Resolvers)
            {
                object result;
                if (resolver(context, targetType, out result))
                    return result;
            }
            throw new RegistryException(context.ActiveResolutionPath, $"Binding failed to resolve an instance of {targetType.FullName}. Resolution Path: {string.Join("->", context.ActiveResolutionPath.Select(x => x.FullName))}");
        }
    }
}