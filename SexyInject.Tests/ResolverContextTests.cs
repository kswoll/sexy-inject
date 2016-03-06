using NUnit.Framework;
using SexyInject.Tests.TestClasses;

namespace SexyInject.Tests
{
    [TestFixture]
    public class ResolverContextTests
    {
        [Test]
        public void Registry()
        {
            var registry = new Registry();
            var binder = registry.Bind<SimpleClass>();
            var context = new ResolverContext(registry, binder, new TestResolver());
            Assert.AreEqual(registry, context.Registry);
        }

        [Test]
        public void Binder()
        {
            var registry = new Registry();
            var binder = registry.Bind<SimpleClass>();
            var context = new ResolverContext(registry, binder, new TestResolver());
            Assert.AreEqual(binder, context.Binder);
        }

        [Test]
        public void BinderT()
        {
            var registry = new Registry();
            var binder = registry.Bind<SimpleClass>();
            var context = new ResolverContext<SimpleClass>(registry, binder, new TestResolver());
            Assert.AreEqual(binder, context.Binder);
        }
    }
}