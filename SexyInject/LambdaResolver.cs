using System;

namespace SexyInject
{
    public class LambdaResolver : IResolver
    {
        private readonly Func<ResolveContext, Type, object> lambda;

        public LambdaResolver(Func<ResolveContext, Type, object> lambda)
        {
            this.lambda = lambda;
        }

        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            result = lambda(context, targetType);
            return true;
        }
    }
}