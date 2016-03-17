using NUnit.Framework;

namespace SexyInject.Tests
{
    [TestFixture]
    public class PropertyBindingTests
    {
        [Test]
        public void PropertyBinding1()
        {
            var registry = new Registry();
            registry.Bind<Dependency>();
            registry.Bind<PropertyClass>(binder => binder.To()
                .InjectProperty(x => x.Dependency)
                .InjectProperty(x => x.String, _ => "foo"));

            var instance = registry.Get<PropertyClass>();
            Assert.IsNotNull(instance.Dependency);
            Assert.AreEqual("foo", instance.String);
        }

        [Test]
        public void PropertyBinding2()
        {
            var registry = new Registry();
            registry.Bind<Dependency>();
            registry.Bind<string>(binder => binder.To(type => "foo"));
            registry.Bind<PropertyClass>(binder => binder.To().InjectProperties());

            var instance = registry.Get<PropertyClass>();
            Assert.IsNotNull(instance.Dependency);
            Assert.AreEqual("foo", instance.String);
        }

        public class PropertyClass 
        {
            public Dependency Dependency { get; set; }
            public string String { get; set; }
        }

        public class Dependency {}
    }
}