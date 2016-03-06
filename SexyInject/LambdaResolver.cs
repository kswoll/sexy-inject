using System;

namespace SexyInject
{
    public class LambdaResolver : IResolver
    {
        private readonly Func<ResolverContext, Type, object> lambda;

        public LambdaResolver(Func<ResolverContext, Type, object> lambda)
        {
            this.lambda = lambda;
        }

        public bool TryResolve(ResolverContext context, Type targetType, out object result)
        {
            result = lambda(context, targetType);
            return true;
        }
    }
}