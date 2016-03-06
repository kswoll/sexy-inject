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

        [Test]
        public void TypeProperty()
        {
            var binder = new Binder(new Registry(), typeof(string));
            Assert.AreEqual(typeof(string), binder.Type);
        }

        [Test]
        public void DefaultResolver()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>();
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void BaseBinderToWithContextAndType()
        {
            var registry = new Registry();
            Binder binder = registry.Bind<SimpleClass>();
            binder.To((context, type) => new SimpleClass());
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void BinderToWithContextAndType()
        {
            var registry = new Registry();
            var binder = registry.Bind<SimpleClass>();
            binder.To((context, type) => new SimpleClass());
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void BaseBinderConstructor()
        {
            var registry = new Registry();
            Binder binder = registry.Bind<SimpleClass>();
            binder.To<SimpleClass>();
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }
    }
}