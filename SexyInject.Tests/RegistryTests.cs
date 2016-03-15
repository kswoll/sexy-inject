using System;
using System.Linq;
using System.Linq.Expressions;
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
            registry.Bind<object>().To(type => registry.Construct(type));
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void ImplicitRegistrationUsingPattern()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void ImplicitRegistrationUsingPatternNotInstantiatable()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            Assert.Throws<RegistryException>(() => registry.Get<ISomeInterface>());
        }

        [Test]
        public void NonGenericGet()
        {
            var registry = new Registry();
            registry.Bind<object>().To(type => registry.Construct(type));
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
            registry.Bind<object>().To(type => registry.Construct(type));
            var injectionClass = registry.Get<InjectionClass>();
            Assert.IsNotNull(injectionClass.SimpleClass);
        }

        [Test]
        public void UnregisteredTypeThrows()
        {
            var registry = new Registry();
            Assert.Throws<RegistryException>(() => registry.Get<SimpleClass>());
        }

        [Test]
        public void ClassWithoutConstructorThrows()
        {
            var registry = new Registry();
            registry.Bind<object>().To(type => registry.Construct(type));
            Assert.Throws<ArgumentException>(() => registry.Get<ClassWithoutConstructor>());
        }

        [Test]
        public void PredicatedResolver()
        {
            var registry = new Registry();
            ((ResolverContext)registry.Bind<ISomeInterface>().To<SomeClass1>()).When((x, y) => false);
            ((ResolverContext)registry.Bind<ISomeInterface>().To<SomeClass2>()).When((x, y) => true);
            var impl = registry.Get<ISomeInterface>();
            Assert.IsTrue(impl is SomeClass2);
        }

        [Test]
        public void PredicatedResolverJustType()
        {
            var registry = new Registry();
            ((ResolverContext)registry.Bind<ISomeInterface>().To<SomeClass1>()).When(_ => false);
            ((ResolverContext)registry.Bind<ISomeInterface>().To<SomeClass2>()).When(_ => true);
            var impl = registry.Get<ISomeInterface>();
            Assert.IsTrue(impl is SomeClass2);
        }

        [Test]
        public void PredicatedResolverT()
        {
            var registry = new Registry();
            registry.Bind<ISomeInterface>().To<SomeClass1>().When((x, y) => false);
            registry.Bind<ISomeInterface>().To<SomeClass2>().When((x, y) => true);
            var impl = registry.Get<ISomeInterface>();
            Assert.IsTrue(impl is SomeClass2);
        }

        [Test]
        public void PredicatedResolverNone()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().When((x, y) => false);
            var impl = registry.Get<SimpleClass>();
            Assert.IsNull(impl);
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
        public void AutoCreateConcreteTypes()
        {
            var registry = new Registry();
            Func<Type, bool> isInstantiatable = type => !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
            registry.Bind<object>().To(type => registry.Construct(type)).When(isInstantiatable);

            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);

            Assert.Throws<RegistryException>(() => registry.Get<ISomeInterface>());
        }

        [Test]
        public void ResolveFactory()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To(x => new SimpleClass { StringProperty = "foo" });
            registry
                .Bind(typeof(Func<>))
                .To((context, targetType) => Expression.Lambda(targetType, registry.GetExpression(targetType.GetGenericArguments()[0])).Compile())
                .Cache((context, targetType) => targetType);
            var factory = registry.Get<Func<SimpleClass>>();
            var simpleClass = factory();
            Assert.AreEqual("foo", simpleClass.StringProperty);
        }

        [Test]
        public void ResolveFactoryUsingPattern()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To(x => new SimpleClass { StringProperty = "foo" });
            registry.RegisterFactoryPattern();
            var factory = registry.Get<Func<SimpleClass>>();
            var simpleClass = factory();
            Assert.AreEqual("foo", simpleClass.StringProperty);
        }

        [Test]
        public void ResolveLazy()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To(x => new SimpleClass { StringProperty = "foo" });
            registry
                .Bind(typeof(Lazy<>))
                .To((context, targetType) =>
                {
                    var returnType = targetType.GetGenericArguments()[0];
                    var lazytype = typeof(Lazy<>).MakeGenericType(returnType);
                    var lambdaType = typeof(Func<>).MakeGenericType(returnType);
                    return Activator.CreateInstance(lazytype, Expression.Lambda(lambdaType, registry.GetExpression(returnType)).Compile());
                })
                .Cache((context, targetType) => targetType);
            var factory = registry.Get<Lazy<SimpleClass>>();
            var simpleClass = factory.Value;
            Assert.AreEqual("foo", simpleClass.StringProperty);
        }

        [Test]
        public void ResolveLazyUsingPattern()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To(x => new SimpleClass { StringProperty = "foo" });
            registry.RegisterLazyPattern();
            var factory = registry.Get<Lazy<SimpleClass>>();
            var simpleClass = factory.Value;
            Assert.AreEqual("foo", simpleClass.StringProperty);
        }

        [Test]
        public void Cache()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To(x => new SimpleClass { StringProperty = "foo" }).Cache((context, targetType) => targetType);
            var instance1 = registry.Get<SimpleClass>();
            var instance2 = registry.Get<SimpleClass>();
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void Construct()
        {
            var registry = new Registry();
            registry.Bind<object>().To(type => registry.Construct(type));
            var simpleClass = registry.Construct<SimpleClass>();
            Assert.IsNotNull(simpleClass);           
        }

        [Test]
        public void ResolveByGenericTypeDefinitionOnInterface()
        {
            var registry = new Registry();
            registry.Bind(typeof(IGenericInterface<>)).To(_ => new ConstructedGenericClass());
            var impl = registry.Get<ConstructedGenericClass>();
            Assert.IsNotNull(impl);
        }

        [Test]
        public void CatchAllForValueType()
        {
            var registry = new Registry();
            registry.Bind(typeof(object)).To(x => 5);
            var value = registry.Get<int>();
            Assert.AreEqual(5, value);
        }

        [Test]
        public void MultipleInjectionOfSameTypeResultsInOneInstance()
        {
            var registry = new Registry();
            registry.Bind<object>().To((context, type) => context.Construct(type));
            var obj = registry.Get<ClassWithDependencyOnOtherClassWithDependencyOnSimpleClass>();
            Assert.AreSame(obj.SimpleClass, obj.ClassWithDependencyOnSimpleClass.SimpleClass);
        }

        [Test]
        public void PassArgument()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var injectionClass = registry.Get<InjectionClass>(simpleClass);
            Assert.AreSame(simpleClass, injectionClass.SimpleClass);
        }

        [Test]
        public void PassNullArgumentThrows()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            Assert.Throws<ArgumentException>(() => registry.Get<InjectionClass>((object)null));
        }

        [Test]
        public void PassDuplicateArgumentThrows()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            Assert.Throws<ArgumentException>(() => registry.Get<InjectionClass>(new SimpleClass(), new SimpleClass()));
        }

        [Test]
        public void SingletonCache()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To<SimpleClass>().Cache((context, targetType) => true);
            var simpleClass1 = registry.Get<SimpleClass>();
            var simpleClass2 = registry.Get<SimpleClass>();
            Assert.AreSame(simpleClass1, simpleClass2);
        }

        [Test]
        public void SingletonTo()
        {
            var registry = new Registry();
            var simpleClass = new SimpleClass();
            Binder binder = registry.Bind<SimpleClass>();
            binder.To(simpleClass);
            var result = registry.Get<SimpleClass>();
            Assert.AreSame(simpleClass, result);
        }

        [Test]
        public void SingletonToT()
        {
            var registry = new Registry();
            var simpleClass = new SimpleClass();
            registry.Bind<SimpleClass>().To(simpleClass);
            var result = registry.Get<SimpleClass>();
            Assert.AreSame(simpleClass, result);
        }

        [Test]
        public void OptOutOfTransientBinding()
        {
            var registry = new Registry();
            registry.Bind<object>().To((context, type) => context.Construct(type));
            registry.Bind<SimpleClass>(CachePolicy.Never);
            var obj = registry.Get<ClassWithDependencyOnOtherClassWithDependencyOnSimpleClass>();
            Assert.AreNotSame(obj.SimpleClass, obj.ClassWithDependencyOnSimpleClass.SimpleClass);
        }

        [Test]
        public void SimpleLambdaBinding()
        {
            var registry = new Registry();
            SimpleClass toSimpleClass = null;
            registry.Bind<SimpleClass>().To(_ => toSimpleClass = new SimpleClass());
            var simpleClass = registry.Get<SimpleClass>();
            Assert.AreSame(toSimpleClass, simpleClass);
        }

        [Test]
        public void ShorthandTo()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To();
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        public void ConstructWithTypeAndArgumentsT()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = registry.Construct<InjectionClass>(new Argument(simpleClass, ArgumentType.Pooled));
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void ConstructWithTypeAndArguments()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = (InjectionClass)registry.Construct(typeof(InjectionClass), new Argument(simpleClass, ArgumentType.Pooled));
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void ConstructWithTypeAndObjectArguments()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = (InjectionClass)registry.Construct(typeof(InjectionClass), simpleClass);
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void ConstructWithTypeAndObjectArgumentsAndConstructorSelector()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = (InjectionClass)registry.Construct(typeof(InjectionClass), x => x.First(), simpleClass);
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void ConstructWithTypeAndArgumentsAndConstructorSelectorUnpooled()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = (InjectionClass)registry.Construct(typeof(InjectionClass), x => x.First(), new Argument(simpleClass, ArgumentType.Unpooled));
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void ClassInjection()
        {
            var registry = new Registry();
            var simpleClass = new SimpleClass();
            registry.Bind<InjectionClass>().To().Inject((context, type) => simpleClass);
            var injectionClass = registry.Get<InjectionClass>();
            Assert.AreSame(simpleClass, injectionClass.SimpleClass);
        }

        [Test]
        public void ClassInjectionJustType()
        {
            var registry = new Registry();
            var simpleClass = new SimpleClass();
            registry.Bind<InjectionClass>().To().Inject(_ => simpleClass);
            var injectionClass = registry.Get<InjectionClass>();
            Assert.AreSame(simpleClass, injectionClass.SimpleClass);
        }

        [Test]
        public void PropertyInjection()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().InjectProperty(x => x.StringProperty, (context, type) => "foo");
            var simpleClass = registry.Get<SimpleClass>();
            Assert.AreEqual("foo", simpleClass.StringProperty);
        }

        [Test]
        public void PropertyInfoInjectionWithMethodThrows()
        {
            var registry = new Registry();
            Assert.Throws<ArgumentException>(() => registry.Bind<SimpleClass>().To().InjectProperty(typeof(SimpleClass).GetMethod(nameof(ToString)), (context, type) => "foo"));
        }

        [Test]
        public void PropertyInfoInjection()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().InjectProperty(typeof(SimpleClass).GetProperty(nameof(SimpleClass.StringProperty)), (context, type) => "foo");
            var simpleClass = registry.Get<SimpleClass>();
            Assert.AreEqual("foo", simpleClass.StringProperty);
        }

        [Test]
        public void PropertyInfoInjectionJustType()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().InjectProperty(typeof(SimpleClass).GetProperty(nameof(SimpleClass.StringProperty)), type => "foo");
            var simpleClass = registry.Get<SimpleClass>();
            Assert.AreEqual("foo", simpleClass.StringProperty);
        }

        [Test]
        public void PropertiesInjection()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            registry.Bind<ISomeInterface>().To<SomeClass1>();
            registry.Bind<SimpleClass>().To().InjectProperties(x => x.SubClass, x => x.SomeInterface);
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass.SomeInterface);
            Assert.IsNotNull(simpleClass.SubClass);
        }

        [Test]
        public void PropertiesInjectionImplicitT()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            registry.Bind<ISomeInterface>().To<SomeClass1>();
            registry.Bind<GenericClass<ISomeInterface>>().To().InjectProperties();
            var simpleClass = registry.Get<GenericClass<ISomeInterface>>();
            Assert.IsNotNull(simpleClass.Property);
            Assert.IsNotNull(simpleClass.SimpleClass);
        }

        [Test]
        public void PropertiesInjectionImplicit()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            registry.Bind<ISomeInterface>().To<SomeClass1>();
            ((Binder)registry.Bind<GenericClass<ISomeInterface>>()).To().InjectProperties();
            var simpleClass = registry.Get<GenericClass<ISomeInterface>>();
            Assert.IsNotNull(simpleClass.Property);
            Assert.IsNotNull(simpleClass.SimpleClass);
        }

        [Test]
        public void InvalidPropertyInjection()
        {
            var registry = new Registry();
            Assert.Throws<ArgumentException>(() => registry.Bind<SimpleClass>().To().InjectProperty(x => null, (context, type) => "foo"));
        }

        [Test]
        public void PredicatedPropertyInjection()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().When(_ => false).InjectProperty(x => x.StringProperty, (context, type) => "foo");
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNull(simpleClass);
        }

        [Test]
        public void PropertyInjectionJustContext()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().InjectProperty(x => x.StringProperty, type => "foo");
            var simpleClass = registry.Get<SimpleClass>();
            Assert.AreEqual("foo", simpleClass.StringProperty);
        }

        [Test]
        public void PropertyInjectionInt()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().InjectProperty(x => x.IntProperty, (context, type) => 5);
            var simpleClass = registry.Get<SimpleClass>();
            Assert.AreEqual(5, simpleClass.IntProperty);
        }

        [Test]
        public void PropertyInjectionImplicit()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().InjectProperty(x => x.SomeInterface);
            registry.Bind<ISomeInterface>().To<SomeClass1>();
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass.SomeInterface);
        }

        [Test]
        public void PropertyInfoInjectionImplicit()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().InjectProperty(typeof(SimpleClass).GetProperty(nameof(SimpleClass.SomeInterface)));
            registry.Bind<ISomeInterface>().To<SomeClass1>();
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsNotNull(simpleClass.SomeInterface);
        }

        [Test]
        public void FieldInjection()
        {
            var registry = new Registry();
            registry.Bind<SimpleClass>().To().InjectProperty(x => x.BoolField, _ => true);
            var simpleClass = registry.Get<SimpleClass>();
            Assert.IsTrue(simpleClass.BoolField);
        }

        [Test]
        public void ReadonlyFieldInjectionThrows()
        {
            var registry = new Registry();
            Assert.Throws<ArgumentException>(() => registry.Bind<SimpleClass>().To().InjectProperty(x => x.ReadonlyField, _ => DateTime.MinValue));
        }

        [Test]
        public void ReadonlyPropertyInjectionThrows()
        {
            var registry = new Registry();
            Assert.Throws<ArgumentException>(() => registry.Bind<SimpleClass>().To().InjectProperty(x => x.ReadonlyProperty, _ => "bar"));
        }

        [Test]
        public void GetTypeWithArguments()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = (InjectionClass)registry.Get(typeof(InjectionClass), simpleClass);
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void GetTypeWithPooledArguments()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = (InjectionClass)registry.Get(typeof(InjectionClass), new Argument(simpleClass, ArgumentType.Pooled));
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void GetTypeWithUnpooledArguments()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = (InjectionClass)registry.Get(typeof(InjectionClass), new Argument(simpleClass, ArgumentType.Unpooled));
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void ConstructTWithArguments()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = registry.Construct<InjectionClass>(simpleClass);
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void ConstructTWithSelector()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var instance = registry.Construct<MultiConstructorClass>(constructors => constructors.Single(x => x.GetParameters()[0].ParameterType == typeof(SimpleClass)));
            Assert.IsNotNull(instance.SimpleClass);
            Assert.IsNull(instance.SomeInterface);
        }

        [Test]
        public void ConstructTWithSelectorAndArguments()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = registry.Construct<MultiConstructorClass>(constructors => constructors.Single(x => x.GetParameters()[0].ParameterType == typeof(SimpleClass)), simpleClass);
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void ConstructTWithSelectorAndUnpooledArguments()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            var simpleClass = new SimpleClass();
            var instance = registry.Construct<MultiConstructorClass>(constructors => constructors.Single(x => x.GetParameters()[0].ParameterType == typeof(SimpleClass)), new Argument(simpleClass, ArgumentType.Pooled));
            Assert.AreSame(simpleClass, instance.SimpleClass);
        }

        [Test]
        public void BindingOnBaseClassOfGenericSubclass()
        {
            var registry = new Registry();
            registry.Bind(typeof(GenericClass<>)).To(type => new GenericSubclass<string>());
            var instance = registry.Get<GenericSubclass<string>>();
            Assert.IsNotNull(instance);
        }
    }
}