using System;
using System.Linq;
using NUnit.Framework;
using SexyInject.Tests.TestClasses;

namespace SexyInject.Tests
{
    [TestFixture]
    public class BinderTests
    {
        [Test]
        public void AddResolverThroughBaseType()
        {
            var registry = new Registry();
            Binder binder = new Binder<SimpleClass>(registry);
            var resolver = new TestResolver();
            binder.AddResolver(resolver);
            Assert.IsTrue(binder.Resolvers.Contains(resolver));
        }

        [Test]
        public void RegistryProperty()
        {
            var registry = new Registry();
            var binder = new Binder<SimpleClass>(registry);
            Assert.AreEqual(registry, binder.Registry);
        }

        public class TestResolver : IResolver
        {
            public bool TryResolve(ResolverContext context, Type targetType, out object result)
            {
                result = new SimpleClass();
                return true;
            }
        }
    }
}