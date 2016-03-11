using System;

namespace SexyInject
{
    /// <summary>
    /// Used to allow you to directly pass in an instance of a class to resolve the dependency.  There's also 
    /// PropertyInjectionResolver, which injects a dependency into a specific property.
    /// </summary>
    public class ClassInjectionResolver : IResolver
    {
        private readonly IResolver resolver;
        private readonly Func<ResolveContext, Type, object> lambda;

        public ClassInjectionResolver(IResolver resolver, Func<ResolveContext, Type, object> lambda)
        {
            this.resolver = resolver;
            this.lambda = lambda;
        }

        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            context.InjectArgument(lambda(context, targetType));
            return resolver.TryResolve(context, targetType, out result);
        }
    }
}