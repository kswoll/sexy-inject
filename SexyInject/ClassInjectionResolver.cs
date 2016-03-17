using System;

namespace SexyInject
{
    /// <summary>
    /// Used to allow you to directly pass in an instance of a class to resolve the dependency.  There's also 
    /// PropertyInjectionResolver, which injects a dependency into a specific property.
    /// </summary>
    public class ClassInjectionResolver : IResolverOperator
    {
        private readonly Func<ResolveContext, Type, object> lambda;

        public ClassInjectionResolver(Func<ResolveContext, Type, object> lambda)
        {
            this.lambda = lambda;
        }

        public bool TryResolve(ResolveContext context, Type targetType, ResolverProcessor resolverProcessor, out object result)
        {
            context.InjectArgument(lambda(context, targetType));
            return resolverProcessor(context, targetType, out result);
        }
    }
}