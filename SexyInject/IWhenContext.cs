using System;

namespace SexyInject
{
    public interface IWhenContext
    {
        IBinder Binder { get; }
        Func<ResolverContext, bool> Predicate { get; }
    }
}