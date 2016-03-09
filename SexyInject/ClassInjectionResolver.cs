using System;

namespace SexyInject
{
    /// <summary>
    /// Used to allow you to directly pass in an instance of a class to resolve the dependency.  There's also 
    /// PropertyInjectionResolver, which injects a dependency into a specific property.
    /// </summary>
    public class ClassInjectionResolver : IResolver
    {
        private readonly Func<ResolveContext, Type, object> lambda;


        public bool TryResolve(ResolveContext context, Type targetType, object[] arguments, out object result)
        {
            throw new NotImplementedException();
        }
    }
}