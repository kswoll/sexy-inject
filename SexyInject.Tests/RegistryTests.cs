using System;
using System.Collections.Generic;
using NUnit.Framework;
using SexyInject.Tests.TestClasses;

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
        public void NonGenericGet()
        {
            var registry = new Registry();
            var simpleClass = registry.Get(typeof(SimpleClass));
            Assert.IsTrue(simpleClass is SimpleClass);
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

        [Test]
        public void UnregisteredTypeThrowsWhenAllowImplicitRegistrationIsFalse()
        {
            var registry = new Registry(false);
            Assert.Throws<RegistryException>(() => registry.Get<SimpleClass>());
        }

        [Test]
        public void ClassWithoutConstructorThrows()
        {
            var registry = new Registry();
            Assert.Throws<ArgumentException>(() => registry.Get<ClassWithoutConstructor>());
        }

        [Test]
        public void PredicatedResolver()
        {
            var registry = new Registry();
            registry.Bind<ISomeInterface>().When((x, y) => false).To<SomeClass1>();
            registry.Bind<ISomeInterface>().When((x, y) => true).To<SomeClass2>();
            var impl = registry.Get<ISomeInterface>();
            Assert.IsTrue(impl is SomeClass2);
        }

        [Test]
        public void LambdaResolver()
        {
            var registry = new Registry();
            registry.Bind<ISomeInterface>().To(_ => new SomeClass1());
            var impl = registry.Get<ISomeInterface>();
            Assert.IsTrue(impl is SomeClass1);
        }

        [Test]
        public void ResolveByGenericTypeDefinition()
        {
            var registry = new Registry();
            registry.Bind(typeof(GenericClass<>)).To(_ => new GenericClass<string> { Property = "1" });
            var impl = registry.Get<GenericClass<string>>();
            Assert.AreEqual("1", impl.Property);
        }

        [Test]
        public void ResolveFactory()
        {
//            var registry = new Registry();
//            registry.Bind(typeof(Func<>)).Cache().To((context, targetType) => () => context.Resolve(targetType));
//            var impl = registry.Get<GenericClass<string>>();
//            Assert.AreEqual("1", impl.Property);            
        }
    }
}