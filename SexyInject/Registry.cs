﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    public class Registry
    {
        public bool AllowImplicitRegistration { get; }

        private static readonly MethodInfo getMethod = typeof(Registry).GetMethods().Single(x => x.Name == nameof(Get) && x.GetParameters().Length == 1);
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

        public Expression GetExpression(Type type)
        {
            return Expression.Convert(Expression.Call(Expression.Constant(this), getMethod, Expression.Constant(type)), type);
        }

        private ResolveContext CreateResolverContext()
        {
            ResolveContext context = null;
            context = new ResolveContext(x => Get(x, context));
            return context;
        }

        private object Get(Type type, ResolveContext context)
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