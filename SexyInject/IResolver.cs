namespace SexyInject
{
    public interface IResolver<out T>
    {
        T Resolve(ResolverContext context);
    }
}