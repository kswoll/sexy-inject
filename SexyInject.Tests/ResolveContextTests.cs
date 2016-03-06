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
    }
}