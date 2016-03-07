using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    public class ResolveContext
    {
        private readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();
        private readonly Func<Type, object> resolver;
        private readonly Func<ResolveContext, Type, Func<ConstructorInfo[], ConstructorInfo>, object> constructor;

        private static readonly MethodInfo resolveMethod = typeof(ResolveContext).GetMethod(nameof(Resolve));

        public ResolveContext(Func<Type, object> resolver, Func<ResolveContext, Type, Func<ConstructorInfo[], ConstructorInfo>, object> constructor, object[] arguments)
        {
            this.resolver = resolver;
            this.constructor = constructor;

            foreach (var argument in arguments)
            {
                if (argument == null)
                    throw new ArgumentException("Arguments array cannot contain null elements.", nameof(arguments));
                var argumentType = argument.GetType();
                if (cache.ContainsKey(argumentType))
                    throw new ArgumentException("Only one argument of the same type may be provided.", nameof(arguments));
                cache[argumentType] = argument;
            }
        }

        public object Resolve(Type type)
        {
            object result;
            if (!cache.TryGetValue(type, out result))
            {
                result = resolver(type);
                cache[type] = result;
            }
            return result;
        }

        public static MethodCallExpression ResolveExpression(Expression resolveContext, Type type)
        {
            return Expression.Call(resolveContext, resolveMethod, Expression.Constant(type));
        }

        public object Construct(Type type, Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
        {
            return constructor(this, type, constructorSelector);
        }

        public T Construct<T>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
        {
            return (T)constructor(this, typeof(T), constructorSelector);
        }
    }
}