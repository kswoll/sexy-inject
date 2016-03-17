using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    /// <summary>
    /// Injects dependencies into a specific property by using a factory function to resolve the dependency.
    /// </summary>
    public class PropertyInjectionResolver : IResolverOperator
    {
        private readonly Action<ResolveContext, object> setter;

        public PropertyInjectionResolver(LambdaExpression property, Func<ResolveContext, Type, object> factory)
            : this(GetMemberInfo(property), factory)
        {
        }

        public PropertyInjectionResolver(MemberInfo memberInfo, Func<ResolveContext, Type, object> factory)
        {
            if (!(memberInfo is FieldInfo) && !(memberInfo is PropertyInfo))
                throw new ArgumentException("Member must specify a property or field.", nameof(memberInfo));

            var contextParameter = Expression.Parameter(typeof(ResolveContext));
            var objectParameter = Expression.Parameter(typeof(object));

            Type targetType = memberInfo.DeclaringType;
            Expression target = Expression.Convert(objectParameter, targetType);
            Expression member = Expression.MakeMemberAccess(target, memberInfo);
            var memberType = (memberInfo as FieldInfo)?.FieldType ?? ((PropertyInfo)memberInfo).PropertyType;

            Expression body = Expression.Assign(member, Expression.Convert(Expression.Invoke(Expression.Constant(factory), 
                contextParameter, Expression.Constant(memberType)), memberType));
            var lambda = Expression.Lambda<Action<ResolveContext, object>>(body, contextParameter, objectParameter);
            setter = lambda.Compile();
        }

        private static MemberInfo GetMemberInfo(LambdaExpression property)
        {
            var memberExpression = property.Body as MemberExpression;
            var memberInfo = memberExpression?.Member;
            if (memberExpression == null || memberInfo == null || property.Parameters.Count != 1 || memberExpression.Expression != property.Parameters[0])
                throw new ArgumentException("Expression must have one parameter and specify a property directly on it.", nameof(property));
            if (!(memberInfo as PropertyInfo)?.CanWrite ?? false)
                throw new ArgumentException($"Property {memberInfo.DeclaringType.FullName}.{memberInfo.Name} has no setter.");
            if ((memberInfo as FieldInfo)?.Attributes.HasFlag(FieldAttributes.InitOnly) ?? false)
                throw new ArgumentException($"Field {memberInfo.DeclaringType.FullName}.{memberInfo.Name} is readonly.");
            return memberInfo;
        }

        public bool TryResolve(ResolveContext context, Type targetType, ResolverProcessor resolverProcessor, out object result)
        {
            if (resolverProcessor(context, targetType, out result))
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