using System;

namespace SexyInject
{
    public class LambdaGlobalResolverOperator : IGlobalResolverOperator
    {
        private readonly Func<ResolverContext, ResolverContext> headOperators;
        private readonly Func<ResolverContext, ResolverContext> tailOperators;

        public LambdaGlobalResolverOperator(Func<ResolverContext, ResolverContext> headOperators = null, Func<ResolverContext, ResolverContext> tailOperators = null)
        {
            this.headOperators = headOperators;
            this.tailOperators = tailOperators;
        }

        public ResolverContext AddHeadOperators(ResolverContext context)
        {
            return headOperators?.Invoke(context) ?? context;
        }

        public ResolverContext AddTailOperators(ResolverContext context)
        {
            return tailOperators?.Invoke(context) ?? context;
        }
    }
}