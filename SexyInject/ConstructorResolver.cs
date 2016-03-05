using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace SexyInject
{
    public class ConstructorResolver<T> : IResolver<T>
    {
        public static ConstructorResolver<T> Instance => instance.Value;

        private static readonly Lazy<ConstructorResolver<T>> instance = new Lazy<ConstructorResolver<T>>(() => new ConstructorResolver<T>(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly Func<ResolverContext, T> constructor;

        public ConstructorResolver()
        {
            var constructor = typeof(T).GetConstructors().OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
            if (constructor == null)
                throw new ArgumentException($"Type {typeof(T).FullName} must have at least one public constructor", nameof(T));

            var parameters = constructor.GetParameters();
            var contextParameter = Expression.Parameter(typeof(ResolverContext), "context");
            var contextResolveMethod = ResolverContext.resolveMethod;
            var arguments = parameters.Select(x => Expression.Call(contextParameter, contextResolveMethod, Expression.Constant(x.ParameterType))).ToArray();
            var body = Expression.New(constructor, arguments);

            var lambda = Expression.Lambda<Func<ResolverContext, T>>(body, contextParameter);
            this.constructor = lambda.Compile();
        }

        public T Resolve(ResolverContext context)
        {
            return constructor(context);
        }
    }
}