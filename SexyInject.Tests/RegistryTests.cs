using NUnit.Framework;

namespace SexyInject.Tests
{
    [TestFixture]
    public class RegistryTests
    {
        [Test]
        public void ImplicitRegistration()
        {
            var registry = new Registry();
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void InterfaceRegistration()
        {
            var registry = new Registry();
            registry.Bind<ISimpleClass>().To<SimpleClass>();
            var simpleClass = registry.Get<ISimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void SimpleInjection()
        {
            var registry = new Registry();
            var injectionClass = registry.Get<InjectionClass>();
            Assert.IsNotNull(injectionClass.SimpleClass);
        }

        public interface ISimpleClass
        {
        }

        public class SimpleClass : ISimpleClass
        {
        }

        public class InjectionClass
        {
            public SimpleClass SimpleClass { get; }

            public InjectionClass(SimpleClass simpleClass)
            {
                SimpleClass = simpleClass;
            }
        }
    }
}