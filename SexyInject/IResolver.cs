using System;

namespace SexyInject
{
    /// <summary>
    /// An interface easily composable with other instances of IResolver using the decorator pattern
    /// to provide fine-grained control of the binding logic.
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Resolves a type based on the provided context.
        /// </summary>
        /// <param name="context">The current resolver context for resolving T.</param>
        /// <param name="targetType"></param>
        /// <param name="result">The object resolved by this resolver.</param>
        /// <returns>True if this resolver successfully resolved the type</returns>
        bool TryResolve(ResolveContext context, Type targetType, out object result);
    }
}