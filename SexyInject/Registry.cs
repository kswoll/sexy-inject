using System;
using System.Collections.Concurrent;

namespace SexyInject
{
    public class Registry
    {
        public bool AllowImplicitRegistration { get; }

        private readonly ConcurrentDictionary<Type, IBinder> binders = new ConcurrentDictionary<Type, IBinder>();

        public Registry(bool allowImplicitRegistration = true)
        {
            AllowImplicitRegistration = allowImplicitRegistration;
        }

        public Binder<T> Bind<T>()
        {
            return (Binder<T>)binders.GetOrAdd(typeof(T), x => new Binder<T>(this));
        }

        public IBinder Bind(Type type)
        {
            return binders.GetOrAdd(type, x => (IBinder)Activator.CreateInstance(typeof(Binder<>).MakeGenericType(type), this));
        }

        public T Get<T>()
        {
            IBinder binder;
            if (!binders.TryGetValue(typeof(T), out binder))
            {
                if (AllowImplicitRegistration)
                {
                    binder = Bind<T>();
                }
                else
                {
                    throw new RegistryException($"The type {typeof(T).FullName} has not been registered and AllowImplicitRegistration is disabled.");
                }
            }
            return (T)binder.Resolve(CreateResolverContext());
        }

        public object Get(Type type)
        {
            return Get(type, CreateResolverContext());
        }

        private ResolverContext CreateResolverContext()
        {
            ResolverContext context = null;
            context = new ResolverContext(x => Get(x, context));
            return context;
        }

        private object Get(Type type, ResolverContext context)
        {
            IBinder binder;
            if (!binders.TryGetValue(type, out binder))
            {
                if (AllowImplicitRegistration)
                {
                    binder = Bind(type);
                }
            }
            return binder.Resolve(context);
        }
    }
}