using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SexyInject
{
    public class Binder 
    {
        public Registry Registry { get; }
        public Type Type { get; }
        public CachePolicy CachePolicy { get; }

        private readonly object locker = new object();
        private readonly ConcurrentQueue<ResolverContext> resolvers = new ConcurrentQueue<ResolverContext>();
        private ConstructorResolver defaultResolver;
        private int defaultResolverCreated;

        public Binder(Registry registry, Type type, CachePolicy cachePolicy)
        {
            Registry = registry;
            Type = type;
            CachePolicy = cachePolicy;
        }

        public ResolverContext AddResolver(IResolver resolver)
        {
            var context = new ResolverContext(Registry, this, resolver);
            resolvers.Enqueue(context);
            return context;
        }

        protected void AddResolverContext(ResolverContext context)
        {
            resolvers.Enqueue(context);
        }

        public IEnumerable<IResolver> Resolvers => resolvers.Select(x => x.Resolver);

        public object Resolve(ResolveContext context, Type targetType, object[] arguments)
        {
            object result;
            foreach (var resolver in Resolvers)
            {
                if (resolver.TryResolve(context, targetType, arguments, out result))
                    return result;
            }
            if (Interlocked.CompareExchange(ref defaultResolverCreated, 0, 1) != 2)
            {
                lock (locker)
                {
                    if (defaultResolver == null)
                    {
                        defaultResolver = new ConstructorResolver(Type);
                        Interlocked.Exchange(ref defaultResolverCreated, 2);                        
                    }
                }
            }
            defaultResolver.TryResolve(context, targetType, arguments, out result);
            return result;
        }

        /// <summary>
        /// Binds requests for T to an instance of TTarget.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) to instantiate when an instance of T is requested.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        public ResolverContext To<TTarget>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
        {
            return AddResolver(new ConstructorResolver(typeof(TTarget)));
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public ResolverContext To<TTarget>(Func<ResolveContext, Type, TTarget> resolver)
        {
            return AddResolver(new LambdaResolver((x, targetType) => resolver(x, targetType)));
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public ResolverContext To<TTarget>(Func<Type, TTarget> resolver)
        {
            return AddResolver(new LambdaResolver((context, targetType) => resolver(targetType)));
        }        

        /// <summary>
        /// Binds requests for T to a specific instance
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="instance">The singleton instance to always return when type T is requested.</param>
        public ResolverContext To<TTarget>(TTarget instance)
        {
            return AddResolver(new LambdaResolver((context, type) => instance));
        }        
    }

    public class Binder<T> : Binder
    {
        public Binder(Registry registry, CachePolicy cachePolicy) : base(registry, typeof(T), cachePolicy)
        {
        }

        public new ResolverContext<T> AddResolver(IResolver resolver)
        {
            var context = new ResolverContext<T>(Registry, this, resolver);
            AddResolverContext(context);
            return context;
        }

        /// <summary>
        /// Binds requests for T to an instance of TTarget.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) to instantiate when an instance of T is requested.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        public new ResolverContext<T> To<TTarget>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
            where TTarget : T
        {
            return AddResolver(new ConstructorResolver(typeof(TTarget)));
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public new ResolverContext<T> To<TTarget>(Func<Type, TTarget> resolver)
            where TTarget : T
        {
            return AddResolver(new LambdaResolver((context, targetType) => resolver(targetType)));
        }        

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public new ResolverContext<T> To<TTarget>(Func<ResolveContext, Type, TTarget> resolver)
            where TTarget : T
        {
            return AddResolver(new LambdaResolver((context, type) => resolver(context, type)));
        }

        /// <summary>
        /// Binds requests for T to a specific instance
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="instance">The singleton instance to always return when type T is requested.</param>
        public new ResolverContext<T> To<TTarget>(TTarget instance)
            where TTarget : T
        {
            return AddResolver(new LambdaResolver((context, type) => instance));
        }

        /// <summary>
        /// Same as To&lt;T&gt; -- but a more concise syntax when you want to resolve to the bound type and apply further
        /// operators.
        /// </summary>
        public ResolverContext<T> To()
        {
            return To<T>();
        }
    }
}