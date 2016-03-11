using System;

namespace SexyInject.Tests.TestClasses
{
    public class TestResolver : IResolver
    {
        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            result = new SimpleClass();
            return true;
        }
    }
}