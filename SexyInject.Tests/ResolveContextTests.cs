using System;
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
        public void ResolveT()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>();
            registry.Bind<InjectionClass>().To((context, type) => new InjectionClass(context.Resolve<SimpleClass>()));
            var injectionClass = registry.Get<InjectionClass>();
            Assert.IsNotNull(injectionClass.SimpleClass);
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

        [Test]
        public void GetParentRequestedType()
        {
            var registry = new Registry();
            Type parent = null;
            registry.Bind<SimpleClass>().To((context, type) =>
            {
                parent = context.GetParentRequestedType(0);
                return new SimpleClass();
            });
            registry.Bind<InjectionClass>();
            registry.Get<InjectionClass>();
            Assert.AreEqual(typeof(InjectionClass), parent);            
        }

        [Test]
        public void GetParentRequestedTypeTooFarReturnsNull()
        {
            var registry = new Registry();
            Type parent = null;
            registry.Bind<SimpleClass>().To((context, type) =>
            {
                parent = context.GetParentRequestedType(1);
                return new SimpleClass();
            });
            registry.Bind<InjectionClass>();
            registry.Get<InjectionClass>();
            Assert.IsNull(parent);            
        }
    }
}