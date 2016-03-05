using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace SexyInject
{
    public class Binder<T> : IBinder
    {
        private Registry registry;
        private Lazy<IResolver<T>> resolver = new Lazy<IResolver<T>>(() => new ConstructorResolver<T>(), LazyThreadSafetyMode.ExecutionAndPublication);

        public Binder(Registry registry)
        {
            this.registry = registry;
        }

        /// <summary>
        /// Binds requests for T to an instance of TTarget.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T to instantiate when an instance of T is requested.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        public void To<TTarget>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
            where TTarget : class, T
        {
            resolver = new Lazy<IResolver<T>>(() => new ConstructorResolver<TTarget>(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public T Resolve(ResolverContext context)
        {
            return resolver.Value.Resolve(context);
        }

        object IBinder.Resolve(ResolverContext context)
        {
            return Resolve(context);
        }
    }
}