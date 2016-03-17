using System;

namespace SexyInject
{
    public interface IResolverOperator
    {
        /// <summary>
        /// Resolves a type based on the provided context.
        /// </summary>
        /// <param name="context">The current resolver context for resolving T.</param>
        /// <param name="targetType">The type for which a resolution is being attempted.</param>
        /// <param name="resolverProcessor">Invoke to proceed to the next operator</param>
        /// <param name="result">The object resolved by this resolver.</param>
        /// <returns>True if this resolver successfully resolved the type</returns>
        bool TryResolve(ResolveContext context, Type targetType, ResolverProcessor resolverProcessor, out object result);         
    }
}