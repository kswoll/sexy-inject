using System;

namespace SexyInject
{
    public class CacheContext
    {
        public Binder Binder { get; }
        public Func<ResolverContext, Type, object> KeySelector { get; }

        public CacheContext(Binder binder, Func<ResolverContext, Type, object> keySelector)
        {
            Binder = binder;
            KeySelector = keySelector;
        }


    }
}