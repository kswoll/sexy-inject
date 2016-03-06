using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SexyInject
{
    public class Registry
    {
        public bool AllowImplicitRegistration { get; }

        private readonly ConcurrentDictionary<Type, Binder> binders = new ConcurrentDictionary<Type, Binder>();

        public Registry(bool allowImplicitRegistration = true)
        {
            AllowImplicitRegistration = allowImplicitRegistration;
        }

        public Binder<T> Bind<T>()
        {
            return (Binder<T>)binders.GetOrAdd(typeof(T), x => new Binder<T>(this));
        }

        public Binder Bind(Type type)
        {
            return binders.GetOrAdd(type, x => new Binder(this, type));
        }

        public T Get<T>()
        {
            Binder binder;
            if (!binders.TryGetValue(typeof(T), out binder))
            {
                var isGenericBinding = typeof(T).IsGenericType && binders.TryGetValue(typeof(T).GetGenericTypeDefinition(), out binder);
                if (!isGenericBinding)
                {
                    if (AllowImplicitRegistration && IsInstantiatable(typeof(T)))
                        binder = Bind<T>();
                    else if (!typeof(T).IsGenericType || !binders.TryGetValue(typeof(T).GetGenericTypeDefinition(), out binder))
                        throw new RegistryException($"The type {typeof(T).FullName} has not been registered and AllowImplicitRegistration is disabled.");                    
                }
            }

            return (T)binder.Resolve(CreateResolverContext(), typeof(T));
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
            Binder binder;
            if (!binders.TryGetValue(type, out binder))
            {
                if (AllowImplicitRegistration)
                {
                    binder = Bind(type);
                }
            }
            return binder.Resolve(context, type);
        }

        private bool IsInstantiatable(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
        }
    }
}