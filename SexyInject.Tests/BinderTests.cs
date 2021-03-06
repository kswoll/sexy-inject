﻿using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SexyInject.Tests.TestClasses;

namespace SexyInject.Tests
{
    [TestFixture]
    public class BinderTests
    {
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
            registry.Bind<SimpleClass>(x => ((Binder)x).To<SimpleClass>());
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void BinderToWithContextAndType()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>(binder => binder.To((context, type) => new SimpleClass()));
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void BaseBinderConstructor()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>(x => ((Binder)x).To<SimpleClass>());
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void CallingBinderMoreThanOnceThrows()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>(x =>
            {
                var result = x.To();
                Assert.Throws<InvalidOperationException>(() => x.To());
                return result;
            });
            registry.Get<SimpleClass>();
        }

        [Test]
        public void BaseBinderToWithConstructorProviderT()
        {
            var registry = new Registry();
            registry.Bind<ISomeInterface>(x => x.To<SomeClass1>());
            registry.Bind<MultiConstructorClass>(binder => ((Binder)binder).To<MultiConstructorClass>(constructors => constructors.Single(x => x.GetParameters()[0].ParameterType.IsInterface)));
            var instance = registry.Get<MultiConstructorClass>();
            Assert.IsNotNull(instance.SomeInterface);
        }

        [Test]
        public void ToWithConstructorProviderT()
        {
            var registry = new Registry();
            registry.Bind<ISomeInterface>(x => x.To<SomeClass1>());
            registry.Bind<MultiConstructorClass>(binder => binder.To<MultiConstructorClass>(constructors => constructors.Single(x => x.GetParameters()[0].ParameterType.IsInterface)));
            var instance = registry.Get<MultiConstructorClass>();
            Assert.IsNotNull(instance.SomeInterface);
        }
    }
}