using System;

namespace SexyInject
{
    public class LambdaGlobalResolverOperator : IGlobalResolverOperator
    {
        private readonly Func<ResolverContext, bool> headOperatorPredicate;
        private readonly Func<ResolverContext, bool> tailOperatorPredicate;
        private readonly Func<ResolverContext, ResolverContext> headOperators;
        private readonly Func<ResolverContext, ResolverContext> tailOperators;

        public LambdaGlobalResolverOperator(Func<ResolverContext, ResolverContext> headOperators = null, Func<ResolverContext, ResolverContext> tailOperators = null,
            Func<ResolverContext, bool> headOperatorPredicate = null, Func<ResolverContext, bool> tailOperatorPredicate = null)
        {
            this.headOperators = headOperators;
            this.tailOperators = tailOperators;
            this.headOperatorPredicate = headOperatorPredicate;
            this.tailOperatorPredicate = tailOperatorPredicate;
        }

        public ResolverContext AddHeadOperators(ResolverContext context)
        {
            if (headOperatorPredicate?.Invoke(context) ?? true)
                return headOperators?.Invoke(context) ?? context;
            else
                return context;
        }

        public ResolverContext AddTailOperators(ResolverContext context)
        {
            if (tailOperatorPredicate?.Invoke(context) ?? true)
                return tailOperators?.Invoke(context) ?? context;
            else
                return context;
        }
    }
}