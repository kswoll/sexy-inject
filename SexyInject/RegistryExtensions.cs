using System;
using System.Linq;
using System.Linq.Expressions;

namespace SexyInject
{
    public static class RegistryExtensions
    {
        /// <summary>
        /// Binds all <see cref="Func{T}" /> to a factory implementation that will create an instance of T for you upon invocation.
        /// </summary>
        /// <param name="registry">The registry on which to apply this pattern.</param>
        public static void RegisterFactoryPattern(this Registry registry)
        {
            registry.Bind(typeof(Func<>), x => x
                .To((context, targetType) => Expression.Lambda(targetType, registry.GetExpression(targetType.GetGenericArguments()[0])).Compile())
                .Cache((context, targetType) => targetType));
        }

        public static void RegisterPartialApplicationPattern(this Registry registry)
        {
            registry.Bind(typeof(Func<,>), x => x
                .To((context, type) =>
                {
                    var constructorType = type.GetGenericArguments()[0];
                    var returnType = type.GetGenericArguments()[1];
                    var constructor = Expression.Parameter(constructorType, "constructor");
                    var lambda = Expression.Lambda(type, registry.PartialApplicationExpression(returnType, constructor), constructor);
                    return lambda.Compile();
                })
                .When(type =>
                {
                    var constructorType = type.GetGenericArguments()[0];
                    var returnType = type.GetGenericArguments()[1];
                    return constructorType.IsGenericType && constructorType.GetGenericTypeDefinition() == typeof(Func<,>) && constructorType.GetGenericArguments()[0] == typeof(ResolveContext) && constructorType.GetGenericArguments()[1] == returnType;
                })
                .Cache(Cache.Singleton));
            registry.Bind(typeof(PartialConstructor<>), x => x
                .To((context, type) =>
                {
                    var returnType = type.GetGenericArguments()[0];
                    var constructorType = typeof(Func<,>).MakeGenericType(typeof(ResolveContext), returnType);
                    var constructor = Expression.Parameter(constructorType, "constructor");
                    var lambda = Expression.Lambda(type, registry.PartialApplicationExpression(returnType, constructor), constructor);
                    return lambda.Compile();
                })
                .Cache(Cache.Singleton));
        }

        /// <summary>
        /// Binds all <see cref="Func{T}" /> to a factory implementation that will create an instance of T for you upon invocation.
        /// </summary>
        /// <param name="registry">The registry on which to apply this pattern.</param>
        public static void RegisterLazyPattern(this Registry registry)
        {
            registry.Bind(typeof(Lazy<>), x => x
                .To((context, targetType) =>
                {
                    var returnType = targetType.GetGenericArguments()[0];
                    var lazytype = typeof(Lazy<>).MakeGenericType(returnType);
                    var lambdaType = typeof(Func<>).MakeGenericType(returnType);
                    return Activator.CreateInstance(lazytype, Expression.Lambda(lambdaType, registry.GetExpression(returnType)).Compile());
                })
                .Cache((context, targetType) => targetType));
        }

        /// <summary>
        /// Allows for instances of a type to be returned from the registry even if that type has not been explicitly bound.
        /// Only applies to instantiatable types (non-interfaces, non-abstract-classes, etc.).
        /// </summary>
        /// <param name="registry">The registry on which to apply this pattern.</param>
        public static void RegisterImplicitPattern(this Registry registry)
        {
            Func<Type, bool> isInstantiatable = type => !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
            registry.Bind<object>(x => x
                .To((context, type) => context.Constructor(type))
                .When((context, targetType) => isInstantiatable(targetType)));
        }

        /// <summary>
        /// Prevents more than one instance of a given type from being returned when resolving a single request.
        /// </summary>
        /// <param name="registry">The registry on which to apply this pattern.</param>
        public static void RegisterTransientCachingPattern(this Registry registry)
        {
            registry.AddGlobalTailOperator(x => x.Cache(Cache.Transient));
        }
    }
}