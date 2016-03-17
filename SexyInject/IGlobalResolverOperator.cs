namespace SexyInject
{
    public interface IGlobalResolverOperator
    {
        ResolverContext AddHeadOperators(ResolverContext context);
        ResolverContext AddTailOperators(ResolverContext context);
    }
}