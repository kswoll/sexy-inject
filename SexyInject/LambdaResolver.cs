using System;

namespace SexyInject
{
    public class LambdaResolver : IResolver
    {
        private readonly Func<ResolverContext, object> lambda;

        public LambdaResolver(Func<ResolverContext, object> lambda)
        {
            this.lambda = lambda;
        }

        public object Resolve(ResolverContext context, out bool isResolved)
        {
            isResolved = true;
            return lambda(context);
        }
    }
}