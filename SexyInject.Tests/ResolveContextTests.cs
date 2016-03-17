using System;
using System.Linq;
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
            registry.Bind<SimpleClass>(x => x.To((context, type) => context.Construct<SimpleClass>()));
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void ConstructTWithArgs()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            registry.Bind<SecondLevelInjectionClass>(x => x.To((context, type) => new SecondLevelInjectionClass(context.Construct<InjectionClass>(simpleClass))));
            var injectionClass = registry.Get<SecondLevelInjectionClass>();
            Assert.AreSame(simpleClass, injectionClass.InjectionClass.SimpleClass);
        }

        [Test]
        public void ConstructTWithArgsAndResolver()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var someClass1 = new SomeClass1();
            registry.Bind<MultiConstructorDependent>(binder => binder.To((context, type) => new MultiConstructorDependent(context.Construct<MultiConstructorClass>(constructors => constructors.Single(x => x.GetParameters()[0].ParameterType == typeof(ISomeInterface)), someClass1))));
            var instance = registry.Get<MultiConstructorDependent>();
            Assert.AreSame(someClass1, instance.MultiConstructorClass.SomeInterface);
        }

        [Test]
        public void ResolveT()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>();
            registry.Bind<InjectionClass>(x => x.To((context, type) => new InjectionClass(context.Resolve<SimpleClass>())));
            var injectionClass = registry.Get<InjectionClass>();
            Assert.IsNotNull(injectionClass.SimpleClass);
        }

        [Test]
        public void ResolveTArgs()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            registry.Bind<SecondLevelInjectionClass>(x => x.To((context, type) => new SecondLevelInjectionClass(context.Resolve<InjectionClass>(simpleClass))));
            var injectionClass = registry.Get<SecondLevelInjectionClass>();
            Assert.AreSame(simpleClass, injectionClass.InjectionClass.SimpleClass);
        }

        [Test]
        public void WrongArgumentIsIgnored()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            registry.Bind<InjectionClass>(x => x.To().Inject((context, type) => new SomeClass1()));
            var injectionClass = registry.Get<InjectionClass>();
            Assert.IsNotNull(injectionClass.SimpleClass);
        }

        [Test]
        public void ResolveTypeArgs()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            registry.Bind<SecondLevelInjectionClass>(x => x.To((context, type) => new SecondLevelInjectionClass((InjectionClass)context.Resolve(typeof(InjectionClass), simpleClass))));
            var injectionClass = registry.Get<SecondLevelInjectionClass>();
            Assert.AreSame(simpleClass, injectionClass.InjectionClass.SimpleClass);
        }

        [Test]
        public void ConstructTWithResolver()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            registry.Bind<ISomeInterface>(x => x.To<SomeClass1>());
            registry.Bind<MultiConstructorDependent>(binder => binder.To((context, type) => new MultiConstructorDependent(context.Construct<MultiConstructorClass>(constructors => constructors.Single(x => x.GetParameters()[0].ParameterType == typeof(ISomeInterface))))));
            var instance = registry.Get<MultiConstructorDependent>();
            Assert.IsNull(instance.MultiConstructorClass.SimpleClass);
            Assert.IsNotNull(instance.MultiConstructorClass.SomeInterface);
        }

        [Test]
        public void ConstructTypeWithResolver()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            registry.Bind<ISomeInterface>(x => x.To<SomeClass1>());
            registry.Bind<MultiConstructorDependent>(binder => binder.To((context, type) => new MultiConstructorDependent((MultiConstructorClass)context.Construct(typeof(MultiConstructorClass), constructors => constructors.Single(x => x.GetParameters()[0].ParameterType == typeof(ISomeInterface))))));
            var instance = registry.Get<MultiConstructorDependent>();
            Assert.IsNull(instance.MultiConstructorClass.SimpleClass);
            Assert.IsNotNull(instance.MultiConstructorClass.SomeInterface);
        }

        [Test]
        public void Registry()
        {
            var registry = new Registry();
            ResolveContext resolveContext = null;
            registry.Bind<SimpleClass>(x => x.To((context, type) =>
            {
                resolveContext = context;
                return new SimpleClass();
            }));
            registry.Get<SimpleClass>();
            Assert.AreSame(registry, resolveContext.Registry);            
        }

        [Test]
        public void GetParentRequestedType()
        {
            var registry = new Registry();
            Type parent = null;
            registry.Bind<SimpleClass>(x => x.To((context, type) =>
            {
                parent = context.GetCallerType(0);
                return new SimpleClass();
            }));
            registry.Bind<InjectionClass>();
            registry.Get<InjectionClass>();
            Assert.AreEqual(typeof(InjectionClass), parent);            
        }

        [Test]
        public void GetParentRequestedTypeTooFarReturnsNull()
        {
            var registry = new Registry();
            Type parent = null;
            registry.Bind<SimpleClass>(x => x.To((context, type) =>
            {
                parent = context.GetCallerType(1);
                return new SimpleClass();
            }));
            registry.Bind<InjectionClass>();
            registry.Get<InjectionClass>();
            Assert.IsNull(parent);            
        }

        [Test]
        public void NullArgumentThrows()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>(x => x.To((context, type) =>
            {
                Assert.Throws<ArgumentNullException>(() => context.InjectArgument(null));
                return new SimpleClass();
            }));
            registry.Get<SimpleClass>();
        }

        [Test]
        public void DuplicateArgumentThrows()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>(x => x.To((context, type) =>
            {
                context.InjectArgument("foo");
                Assert.Throws<ArgumentException>(() => context.InjectArgument("bar"));
                return new SimpleClass();
            }));
            registry.Get<SimpleClass>();
        }
    }
}