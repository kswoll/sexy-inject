using System;
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
            registry.Bind<SimpleClass>(binder =>
            {
                var context = binder.To();
                Assert.AreEqual(registry, context.Registry);
                return context;
            });
        }

        [Test]
        public void ResolverProcessorCalledMoreThanOnceThrows()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>(x => x.To().AddOperator(new TestOperator()));
            Assert.Throws<InvalidOperationException>(() => registry.Get<SimpleClass>());
        }

        private class TestOperator : IResolverOperator
        {
            public bool TryResolve(ResolveContext context, Type targetType, ResolverProcessor resolverProcessor, out object result)
            {
                resolverProcessor(context, targetType, out result);
                return resolverProcessor(context, targetType, out result);
            }
        }

/*
        [Test]
        public void Binder()
        {
            var registry = new Registry();
            var binder = registry.Bind<SimpleClass>();
            var context = new ResolverContext(registry, binder, new TestResolver());
            Assert.AreEqual(binder, context.Binding);
        }
*/
/*

        [Test]
        public void BinderT()
        {
            var registry = new Registry();
            var binder = registry.Bind<SimpleClass>();
            var context = new ResolverContext<SimpleClass>(registry, binder, new TestResolver());
            Assert.AreEqual(binder, context.Binding);
        }
*/
    }
}