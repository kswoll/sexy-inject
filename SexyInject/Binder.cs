using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace SexyInject
{
    public class Binder<T> : IBinder
    {
        public Registry Registry { get; }

        private readonly object locker = new object();
        private readonly ConcurrentQueue<IResolver<T>> resolvers = new ConcurrentQueue<IResolver<T>>();
        private ConstructorResolver<T> defaultResolver;
        private int defaultResolverCreated;

        public Binder(Registry registry)
        {
            Registry = registry;
        }

        public void AddResolver(IResolver<T> resolver)
        {
            resolvers.Enqueue(resolver);
        }

        public IEnumerable<IResolver<T>> Resolvers => resolvers;

        public WhenContext<T> When(Func<ResolverContext, bool> predicate)
        {
            return new WhenContext<T>(this, predicate);
        }

        public T Resolve(ResolverContext context)
        {
            bool isResolved;
            foreach (var resolver in resolvers)
            {
                var result = resolver.Resolve(context, out isResolved);
                if (isResolved)
                    return result;
            }
            if (Interlocked.CompareExchange(ref defaultResolverCreated, 0, 1) != 2)
            {
                lock (locker)
                {
                    defaultResolver = new ConstructorResolver<T>();
                    Interlocked.Exchange(ref defaultResolverCreated, 2);
                }
            }
            return defaultResolver.Resolve(context, out isResolved);
        }

        object IBinder.Resolve(ResolverContext context)
        {
            return Resolve(context);
        }

        void IBinder.AddResolver(IResolver resolver)
        {
            AddResolver((IResolver<T>)resolver);
        }

        /// <summary>
        /// Binds requests for T to an instance of TTarget.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) to instantiate when an instance of T is requested.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        public void To<TTarget>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
            where TTarget : class, T
        {
            AddResolver(new ConstructorResolver<TTarget>());
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public void To<TTarget>(Func<ResolverContext, TTarget> resolver)
            where TTarget : class, T
        {
            AddResolver(new LambdaResolver<TTarget>(resolver));
        }
    }
}