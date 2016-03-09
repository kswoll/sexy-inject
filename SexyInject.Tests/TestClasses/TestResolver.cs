using System;

namespace SexyInject.Tests.TestClasses
{
    public class TestResolver : IResolver
    {
        public bool TryResolve(ResolveContext context, Type targetType, object[] arguments, out object result)
        {
            result = new SimpleClass();
            return true;
        }
    }
}