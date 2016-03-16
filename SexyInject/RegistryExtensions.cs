using System;
using System.Linq.Expressions;

namespace SexyInject
{
    public static class RegistryExtensions
    {
        /// <summary>
        /// Binds all <see cref="Func{T}" /> to a factory implementation that will create an instance of T for you upon invocation.
        /// </summary>
        public static void RegisterFactoryPattern(this Registry registry)
        {
            registry
                .Bind(typeof(Func<>))
                .To((context, targetType) => Expression.Lambda(targetType, registry.GetExpression(targetType.GetGenericArguments()[0])).Compile())
                .Cache((context, targetType) => targetType);
        }

        /// <summary>
        /// Binds all <see cref="Func{T}" /> to a factory implementation that will create an instance of T for you upon invocation.
        /// </summary>
        public static void RegisterLazyPattern(this Registry registry)
        {
            registry
                .Bind(typeof(Lazy<>))
                .To((context, targetType) =>
                {
                    var returnType = targetType.GetGenericArguments()[0];
                    var lazytype = typeof(Lazy<>).MakeGenericType(returnType);
                    var lambdaType = typeof(Func<>).MakeGenericType(returnType);
                    return Activator.CreateInstance(lazytype, Expression.Lambda(lambdaType, registry.GetExpression(returnType)).Compile());
                })
                .Cache((context, targetType) => targetType);            
        }

        /// <summary>
        /// Allows for instances of a type to be returned from the registry even if that type has not been explicitly bound.
        /// Only applies to instantiatable types (non-interfaces, non-abstract-classes, etc.).
        /// </summary>
        /// <param name="registry"></param>
        public static void RegisterImplicitPattern(this Registry registry)
        {
            Func<Type, bool> isInstantiatable = type => !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
            registry.Bind<object>().To((context, type) => context.Constructor(type)).When((context, targetType) => isInstantiatable(targetType));
        }
    }
}