using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    /// <summary>
    /// Instantiates type T using a particular constructor.  It generates a labda to do this rather than use ConstructorInfo.Invoke
    /// for performance benefits.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConstructorResolver : IResolver
    {
        private readonly Func<ResolverContext, object> constructor;

        public ConstructorResolver(Type type, Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
        {
            constructorSelector = constructorSelector ?? (constructors => constructors.OrderByDescending(x => x.GetParameters().Length).FirstOrDefault());

            var constructor = constructorSelector(type.GetConstructors());
            if (constructor == null)
                throw new ArgumentException($"Type {type.FullName} must have at least one public constructor", nameof(type));

            var parameters = constructor.GetParameters();
            var contextParameter = Expression.Parameter(typeof(ResolverContext), "context");
            var contextResolveMethod = ResolverContext.resolveMethod;

            // context.Resolve(arg0Type), context.Resolve(arg1Type)...
            var arguments = parameters.Select(x => Expression.Convert(Expression.Call(contextParameter, contextResolveMethod, Expression.Constant(x.ParameterType)), x.ParameterType)).ToArray();

            // new T(arguments)
            var body = Expression.New(constructor, arguments);

            // context => body
            var lambda = Expression.Lambda<Func<ResolverContext, object>>(body, contextParameter);

            // Compile it into a delegate we can actually invoke
            this.constructor = lambda.Compile();
        }

        public object Resolve(ResolverContext context, out bool isResolved)
        {
            isResolved = true;
            return constructor(context);
        }
    }
}