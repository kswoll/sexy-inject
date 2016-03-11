using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    public class PropertyInjectionResolver : IResolver
    {
        private readonly IResolver resolver;
        private readonly Action<ResolveContext, Type, object> setter;

        public PropertyInjectionResolver(IResolver resolver, LambdaExpression property, Func<ResolveContext, Type, object> factory)
        {
            this.resolver = resolver;

            var contextParameter = Expression.Parameter(typeof(ResolveContext));
            var typeParameter = Expression.Parameter(typeof(Type));
            var objectParameter = Expression.Parameter(typeof(object));
            var memberExpression = property.Body as MemberExpression;
            var memberInfo = memberExpression?.Member;
            if (memberExpression == null || memberInfo == null || property.Parameters.Count != 1 || memberExpression.Expression != property.Parameters[0])
                throw new ArgumentException("Expression must have one parameter specify a property directly on it.", nameof(property));

            Type targetType = property.Parameters.Single().Type;
            Expression target = Expression.Convert(objectParameter, targetType);
            Expression member = Expression.MakeMemberAccess(target, memberInfo);

            Expression body = Expression.Assign(member, Expression.Convert(Expression.Invoke(Expression.Constant(factory), contextParameter, typeParameter), memberExpression.Type));
            var lambda = Expression.Lambda<Action<ResolveContext, Type, object>>(body, contextParameter, typeParameter, objectParameter);
            setter = lambda.Compile();
        }

        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            if (resolver.TryResolve(context, targetType, out result))
            {
                setter(context, targetType, result);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}