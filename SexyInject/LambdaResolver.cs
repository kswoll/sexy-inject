using System;

namespace SexyInject
{
    public class LambdaResolver<TTarget> : IResolver<TTarget>
    {
        private readonly Func<ResolverContext, TTarget> lambda;

        public LambdaResolver(Func<ResolverContext, TTarget> lambda)
        {
            this.lambda = lambda;
        }

        public TTarget Resolve(ResolverContext context, out bool isResolved)
        {
            isResolved = true;
            return lambda(context);
        }
    }
}