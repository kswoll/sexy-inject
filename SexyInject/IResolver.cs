namespace SexyInject
{
    public interface IResolver
    {
        /// <summary>
        /// Resolves T based on the provided context.
        /// 
        /// Note, we use a different pattern than the usual TryXXX methods in order to maintain covariance on T (If 
        /// T were an out parameter, it'd violate the rule)
        /// </summary>
        /// <param name="context">The current resolver context for resolving T.</param>
        /// <param name="isResolved">Should be set to true if the type was successfully resolved.</param>
        /// <returns></returns>
        object Resolve(ResolverContext context, out bool isResolved);
    }
}