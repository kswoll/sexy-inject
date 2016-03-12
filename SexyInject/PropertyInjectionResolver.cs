using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    /// <summary>
    /// Injects dependencies into a specific property by using a factory function to resolve the dependency.
    /// </summary>
    public class PropertyInjectionResolver : IResolver
    {
        private readonly IResolver resolver;
        private readonly Action<ResolveContext, object> setter;

        public PropertyInjectionResolver(IResolver resolver, LambdaExpression property, Func<ResolveContext, Type, object> factory)
        {
            this.resolver = resolver;

            var contextParameter = Expression.Parameter(typeof(ResolveContext));
            var objectParameter = Expression.Parameter(typeof(object));
            var memberExpression = property.Body as MemberExpression;
            var memberInfo = memberExpression?.Member;
            if (memberExpression == null || memberInfo == null || property.Parameters.Count != 1 || memberExpression.Expression != property.Parameters[0])
                throw new ArgumentException("Expression must have one parameter and specify a property directly on it.", nameof(property));
            if (!(memberInfo as PropertyInfo)?.CanWrite ?? false)
                throw new ArgumentException($"Property {memberInfo.DeclaringType.FullName}.{memberInfo.Name} has no setter.");
            if ((memberInfo as FieldInfo)?.Attributes.HasFlag(FieldAttributes.InitOnly) ?? false)
                throw new ArgumentException($"Field {memberInfo.DeclaringType.FullName}.{memberInfo.Name} is readonly.");

            Type targetType = property.Parameters.Single().Type;
            Expression target = Expression.Convert(objectParameter, targetType);
            Expression member = Expression.MakeMemberAccess(target, memberInfo);

            Expression body = Expression.Assign(member, Expression.Convert(Expression.Invoke(Expression.Constant(factory), contextParameter, Expression.Constant(memberExpression.Type)), memberExpression.Type));
            var lambda = Expression.Lambda<Action<ResolveContext, object>>(body, contextParameter, objectParameter);
            setter = lambda.Compile();
        }

        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            if (resolver.TryResolve(context, targetType, out result))
            {
                setter(context, result);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}