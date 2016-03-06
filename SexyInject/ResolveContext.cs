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

        public ResolveContext(Func<Type, object> resolver, Func<ResolveContext, Type, Func<ConstructorInfo[], ConstructorInfo>, object> constructor)
        {
            this.resolver = resolver;
            this.constructor = constructor;
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