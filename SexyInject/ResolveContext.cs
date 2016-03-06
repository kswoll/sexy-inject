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

        private static readonly MethodInfo resolveMethod = typeof(ResolveContext).GetMethod(nameof(Resolve));

        public ResolveContext(Func<Type, object> resolver)
        {
            this.resolver = resolver;
        }

        public object Resolve(Type type)
        {
            return resolver(type);
        }

        public static MethodCallExpression ResolveExpression(Expression resolveContext, Type type)
        {
            return Expression.Call(resolveContext, resolveMethod, Expression.Constant(type));
        }
    }
}