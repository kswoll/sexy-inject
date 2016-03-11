﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    public class Registry
    {
        private static readonly MethodInfo getMethod = typeof(Registry).GetMethods().Single(x => x.Name == nameof(Get) && x.GetParameters().Length == 2 && x.GetParameters()[1].ParameterType == typeof(Argument[]));
        private readonly ConcurrentDictionary<Type, Binder> binders = new ConcurrentDictionary<Type, Binder>();
        private readonly ConcurrentDictionary<Tuple<Type, ConstructorSelector>, Func<ResolveContext, object>> factoryCache = new ConcurrentDictionary<Tuple<Type, ConstructorSelector>, Func<ResolveContext, object>>();

        public Binder<T> Bind<T>(CachePolicy cachePolicy = CachePolicy.Transient)
        {
            return (Binder<T>)binders.GetOrAdd(typeof(T), x => new Binder<T>(this, cachePolicy));
        }

        public Binder Bind(Type type, CachePolicy cachePolicy = CachePolicy.Transient)
        {
            return binders.GetOrAdd(type, x => new Binder(this, type, cachePolicy));
        }

        public T Get<T>()
        {
            return Get<T>(new Argument[0]);
        }

        public object Get(Type type)
        {
            return Get(type, new Argument[0]);
        }

        public T Get<T>(params Argument[] arguments)
        {
            return (T)Get(typeof(T), arguments);
        }

        public T Get<T>(params object[] arguments)
        {
            return (T)Get(typeof(T), arguments.ToArguments(ArgumentType.Pooled));
        }

        public object Get(Type type, params object[] arguments)
        {
            return Get(type, arguments.ToArguments(ArgumentType.Pooled));
        }

        public object Get(Type type, params Argument[] arguments)
        {
            return Get(CreateResolverContext(arguments.Where(x => x.ArgumentType == ArgumentType.Pooled).Select(x => x.Value)), type, arguments.Where(x => x.ArgumentType == ArgumentType.Unpooled).Select(x => x.Value).ToArray());
        }

        public Expression GetExpression(Type type)
        {
            return Expression.Convert(Expression.Call(Expression.Constant(this), getMethod, Expression.Constant(type), Expression.Constant(new Argument[0])), type);
        }

        public T Construct<T>()
        {
            return (T)Construct(typeof(T), null, new Argument[0]);
        }

        public T Construct<T>(ConstructorSelector constructorSelector)
        {
            return (T)Construct(typeof(T), constructorSelector, new Argument[0]);
        }

        public T Construct<T>(params Argument[] arguments)
        {
            return (T)Construct(typeof(T), null, arguments);
        }

        public T Construct<T>(params object[] arguments)
        {
            return (T)Construct(typeof(T), null, arguments);
        }

        public T Construct<T>(ConstructorSelector constructorSelector, params Argument[] arguments)
        {
            return (T)Construct(typeof(T), constructorSelector, arguments);
        }

        public T Construct<T>(ConstructorSelector constructorSelector, params object[] arguments)
        {
            return (T)Construct(typeof(T), constructorSelector, arguments);
        }

        public object Construct(Type type)
        {
            return Construct(type, null, new Argument[0]);
        }

        public object Construct(Type type, params Argument[] arguments)
        {
            return Construct(type, null, arguments);
        }

        public object Construct(Type type, params object[] arguments)
        {
            return Construct(type, null, arguments.ToArguments(ArgumentType.Pooled));
        }

        public object Construct(Type type, ConstructorSelector constructorSelector, params object[] arguments)
        {
            return Construct(type, constructorSelector, arguments.ToArguments(ArgumentType.Pooled));
        }

        public object Construct(Type type, ConstructorSelector constructorSelector, params Argument[] arguments)
        {
            return Construct(CreateResolverContext(arguments.Where(x => x.ArgumentType == ArgumentType.Pooled).Select(x => x.Value)), type, constructorSelector, arguments.Where(x => x.ArgumentType == ArgumentType.Unpooled).Select(x => x.Value).ToArray());
        }

        private object Construct(ResolveContext context, Type type, ConstructorSelector constructorSelector, object[] arguments)
        {
            return context.Construct(type, constructorSelector, arguments);
        }

        private ResolveContext CreateResolverContext(IEnumerable<object> arguments)
        {
            ResolveContext context = null;
            context = new ResolveContext(
                this,
                x => Resolve(context, x), 
                (type, constructorSelector) => factoryCache.GetOrAdd(Tuple.Create(type, constructorSelector), x => FactoryGenerator(x.Item1, x.Item2))(context)
                , arguments);
            return context;
        }

        private object Resolve(ResolveContext context, Type type)
        {
            Binder binder = null;
            foreach (var current in EnumerateBaseTypes(type))
            {
                if (binders.TryGetValue(current, out binder))
                    break;
            }
            if (binder == null)
                throw new RegistryException($"The type {type.FullName} has not been registered and AllowImplicitRegistration is disabled.");

            return binder.Resolve(context, type);            
        }

        private object Get(ResolveContext context, Type type, object[] arguments)
        {
            return context.Resolve(type, arguments);
        }

        private Func<ResolveContext, object> FactoryGenerator(Type type, ConstructorSelector constructorSelector)
        {
            constructorSelector = constructorSelector ?? (constructors => constructors.OrderByDescending(x => x.GetParameters().Length).FirstOrDefault());

            var constructor = constructorSelector(type.GetConstructors());
            if (constructor == null)
                throw new ArgumentException($"Type {type.FullName} must have at least one public constructor", nameof(type));

            var parameters = constructor.GetParameters();
            var contextParameter = Expression.Parameter(typeof(ResolveContext), "context");

            // context.TryResolve(arg0Type), context.TryResolve(arg1Type)...
            var arguments = parameters.Select(x => Expression.Convert(ResolveContext.ResolveExpression(contextParameter, x.ParameterType, Expression.Constant(new object[0])), x.ParameterType)).ToArray();

            // new T(arguments)
            var body = Expression.New(constructor, arguments);

            // context => body
            var lambda = Expression.Lambda<Func<ResolveContext, object>>(body, contextParameter);

            // Compile it into a delegate we can actually invoke
            return lambda.Compile();
        }

        private IEnumerable<Type> EnumerateBaseTypes(Type type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;

                if (current.IsGenericType)
                    yield return current.GetGenericTypeDefinition();

                current = current.BaseType;
            }
            foreach (var @interface in type.GetInterfaces())
            {
                yield return @interface;
                if (@interface.IsGenericType)
                    yield return @interface.GetGenericTypeDefinition();
            }
        }
    }
}