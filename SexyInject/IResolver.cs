namespace SexyInject
{
    public interface IResolver
    {
        /// <summary>
        /// Resolves a type based on the provided context.
        /// </summary>
        /// <param name="context">The current resolver context for resolving T.</param>
        /// <param name="result">The object resolved by this resolver.</param>
        /// <returns>True if this resolver successfully resolved the type</returns>
        bool TryResolve(ResolverContext context, out object result);
    }
}