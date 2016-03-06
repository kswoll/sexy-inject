using System;
using System.Reflection;

namespace SexyInject
{
    public struct WhenContext : IWhenContext
    {
        public Binder Binder { get; }
        public Func<ResolverContext, bool> Predicate { get; }

        public WhenContext(Binder binder, Func<ResolverContext, bool> predicate)
        {
            Binder = binder;
            Predicate = predicate;
        }

        IBinder IWhenContext.Binder => Binder;

        /// <summary>
        /// Binds requests for T to an instance of TTarget only on the condition that it passes the predicate.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T to instantiate when an instance of T is requested.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        public void To<TTarget>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
        {
            Binder.AddResolver(new PredicatedResolver(Predicate, new ConstructorResolver(typeof(TTarget), constructorSelector)));
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function only on the condition that it passes the predicate.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public void To<TTarget>(Func<ResolverContext, TTarget> resolver)
        {
            Binder.AddResolver(new LambdaResolver(x => resolver(x)));
        }        
    }

    public struct WhenContext<T> : IWhenContext
    {
        public Binder<T> Binder { get; }
        public Func<ResolverContext, bool> Predicate { get; }

        public WhenContext(Binder<T> binder, Func<ResolverContext, bool> predicate)
        {
            Binder = binder;
            Predicate = predicate;
        }

        IBinder IWhenContext.Binder => Binder;

        /// <summary>
        /// Binds requests for T to an instance of TTarget only on the condition that it passes the predicate.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T to instantiate when an instance of T is requested.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        public void To<TTarget>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
            where TTarget : class, T
        {
            Binder.AddResolver(new PredicatedResolver(Predicate, new ConstructorResolver(typeof(TTarget), constructorSelector)));
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function only on the condition that it passes the predicate.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public void To<TTarget>(Func<ResolverContext, TTarget> resolver)
            where TTarget : class, T
        {
            Binder.AddResolver(new LambdaResolver(resolver));
        }        
    }
}