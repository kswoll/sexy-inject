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

        public bool TryResolve(ResolverContext context, out object result)
        {
            result = lambda(context);
            return true;
        }
    }
}