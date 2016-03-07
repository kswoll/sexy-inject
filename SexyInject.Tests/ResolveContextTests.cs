using NUnit.Framework;
using SexyInject.Tests.TestClasses;

namespace SexyInject.Tests
{
    [TestFixture]
    public class ResolveContextTests
    {
        [Test]
        public void ConstructT()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To((context, type) => context.Construct<SimpleClass>());
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void Registry()
        {
            var registry = new Registry();
            ResolveContext resolveContext = null;
            registry.Bind<SimpleClass>().To((context, type) =>
            {
                resolveContext = context;
                return new SimpleClass();
            });
            registry.Get<SimpleClass>();
            Assert.AreSame(registry, resolveContext.Registry);            
        }
    }
}