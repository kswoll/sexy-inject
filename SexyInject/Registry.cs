﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SexyInject
{
    public class Registry
    {
        public bool AllowImplicitRegistration { get; }

        private static readonly MethodInfo genericBindMethod = typeof(Registry).GetMethods().Single(x => x.Name == nameof(Bind) && x.GetParameters().Length == 0);
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
            return (IBinder)genericBindMethod.MakeGenericMethod(type).Invoke(this, new object[0]);
        }

        public T Get<T>()
        {
            IBinder binder;
            if (!binders.TryGetValue(typeof(T), out binder))
            {
                if (AllowImplicitRegistration && IsInstantiatable(typeof(T)))
                    binder = Bind<T>();
                else if (!typeof(T).IsGenericType || !binders.TryGetValue(typeof(T).GetGenericTypeDefinition(), out binder))
                    throw new RegistryException($"The type {typeof(T).FullName} has not been registered and AllowImplicitRegistration is disabled.");
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

        private bool IsInstantiatable(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
        }
    }
}