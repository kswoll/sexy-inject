﻿using System.Linq;
using NUnit.Framework;
using SexyInject.Tests.TestClasses;

namespace SexyInject.Tests
{
    [TestFixture]
    public class BinderTests
    {
        [Test]
        public void AddResolverThroughInterface()
        {
            var registry = new Registry();
            var binder = new Binder<SimpleClass>(registry);
            var resolver = new TestResolver();
            ((IBinder)binder).AddResolver(resolver);
            Assert.IsTrue(binder.Resolvers.Contains(resolver));
        }

        [Test]
        public void RegistryProperty()
        {
            var registry = new Registry();
            var binder = new Binder<SimpleClass>(registry);
            Assert.AreEqual(registry, binder.Registry);
        }

        public class TestResolver : IResolver<SimpleClass>
        {
            public SimpleClass Resolve(ResolverContext context, out bool isResolved)
            {
                isResolved = true;
                return new SimpleClass();
            }
        }
    }
}