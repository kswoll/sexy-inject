using System;
using System.Collections.Generic;
using System.Threading;

namespace SexyInject
{
    public class Binder<T> : IBinder
    {
        private Lazy<IResolver<T>> resolver = new Lazy<IResolver<T>>(() => ConstructorResolver<T>.Instance, LazyThreadSafetyMode.ExecutionAndPublication);

        public void To<TTarget>()
            where TTarget : class, T
        {
            resolver = new Lazy<IResolver<T>>(() => ConstructorResolver<TTarget>.Instance, LazyThreadSafetyMode.ExecutionAndPublication);
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