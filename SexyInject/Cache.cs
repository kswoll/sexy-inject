using System;

namespace SexyInject
{
    public static class Cache
    {
        public static Func<ResolveContext, Type, object> Singleton { get; } = (context, type) => true;
        public static Func<ResolveContext, Type, object> ByType { get; } = (context, type) => type;
        public static Func<ResolveContext, Type, object> Transient { get; } = (context, type) => Tuple.Create(context, type);
    }
}