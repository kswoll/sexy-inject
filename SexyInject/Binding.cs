using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SexyInject
{
    public class Binding 
    {
        public Registry Registry { get; }
        public Type Type { get; }

        internal IEnumerable<ResolverContext> ResolverContexts => resolverContexts;

        private readonly object locker = new object();
        private readonly ConcurrentQueue<ResolverContext> resolverContexts = new ConcurrentQueue<ResolverContext>();

        public Binding(Registry registry, Type type)
        {
            Registry = registry;
            Type = type;
        }

        public void AddResolverContext(ResolverContext context)
        {
            resolverContexts.Enqueue(context);
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
            throw new RegistryException($"Binding failed to resolve an instance of {targetType.FullName}");
        }
    }
}