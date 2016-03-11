using System;
using System.Linq;

namespace SexyInject
{
    /// <summary>
    /// Instantiates type T using a particular constructor.  It generates a labda to do this rather than use ConstructorInfo.Invoke
    /// for performance benefits.
    /// </summary>
    public class ConstructorResolver : IResolver
    {
        private readonly Type type;
        private readonly ConstructorSelector constructorSelector;

        public ConstructorResolver(Type type, ConstructorSelector constructorSelector = null)
        {
            this.type = type;
            this.constructorSelector = constructorSelector;
        }

        public bool TryResolve(ResolveContext context, Type targetType, out object result)
        {
            result = context.Constructor(type, constructorSelector);
            return true;
        }
    }
}