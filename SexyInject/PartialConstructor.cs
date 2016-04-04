using System;

namespace SexyInject
{
    public delegate T PartialConstructor<T>(Func<ResolveContext, T> factory);
}