using System;

namespace SexyInject
{
    public delegate bool ResolverProcessor(ResolveContext context, Type targetType, out object result);
}