using NUnit.Framework;

namespace SexyInject.Tests
{
    [TestFixture]
    public class OrderOfOperationsTests
    {
        [Test]
        public void IncrementThenCache()
        {
            var registry = new Registry();
            registry.Bind<PropertyClass>(binder => binder.To().Cache(Cache.Singleton).WhenResolved(x => x.Integer++));
            registry.Get<PropertyClass>();
            var instance = registry.Get<PropertyClass>();
            Assert.AreEqual(2, instance.Integer);
        }

        [Test]
        public void CacheThenIncrement()
        {
            var registry = new Registry();
            registry.Bind<PropertyClass>(binder => binder.To().WhenResolved(x => x.Integer++).Cache(Cache.Singleton));
            registry.Get<PropertyClass>();
            var instance = registry.Get<PropertyClass>();
            Assert.AreEqual(1, instance.Integer);
        }

        public class PropertyClass 
        {
            public int Integer { get; set; }
        }

    }
}