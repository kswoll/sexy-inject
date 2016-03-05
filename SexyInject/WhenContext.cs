using System;
using System.Reflection;

namespace SexyInject
{
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
            Binder.AddResolver(new PredicatedResolver<T>(Predicate, new ConstructorResolver<TTarget>(constructorSelector)));
        }
    }
}