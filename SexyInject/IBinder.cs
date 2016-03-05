namespace SexyInject
{
    public interface IBinder
    {
        object Resolve(ResolverContext context);
    }
}