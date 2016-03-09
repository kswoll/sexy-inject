using System;
using System.Linq;
using System.Reflection;

namespace SexyInject
{
    /// <summary>
    /// Instantiates type T using a particular constructor.  It generates a labda to do this rather than use ConstructorInfo.Invoke
    /// for performance benefits.
    /// </summary>
    public class ConstructorResolver : IResolver
    {
        private readonly Type type;
        private readonly Func<ConstructorInfo[], ConstructorInfo> constructorSelector;

        public ConstructorResolver(Type type, Func<ConstructorInfo[], ConstructorInfo> constructorSelector = null)
        {
            this.type = type;
            this.constructorSelector = constructorSelector ?? (constructors => constructors.OrderByDescending(x => x.GetParameters().Length).FirstOrDefault());
        }

        public bool TryResolve(ResolveContext context, Type targetType, object[] arguments, out object result)
        {
            result = context.Construct(type, constructorSelector, arguments);
            return true;
        }
    }
}