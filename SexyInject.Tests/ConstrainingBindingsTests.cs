using NUnit.Framework;

namespace SexyInject.Tests
{
    [TestFixture]
    public class ConstrainingBindingsTests
    {
        [Test]
        public void TypeSpecificInjection()
        {
            var registry = new Registry();
            registry.RegisterImplicitPattern();
            registry
                .Bind<StringClass>()
                .To(_ => new StringClass("value1"))
                .When((context, targetType) => context.CallerType == typeof(Consumer1));
            registry
                .Bind<StringClass>()
                .To(_ => new StringClass("value2"))
                .When((context, targetType) => context.CallerType == typeof(Consumer2));

            var consumer1 = registry.Get<Consumer1>();
            var consumer2 = registry.Get<Consumer2>();

            Assert.AreEqual("value1", consumer1.StringClass.StringProperty);
            Assert.AreEqual("value2", consumer2.StringClass.StringProperty);
        }

        public class StringClass
        {
            public string StringProperty { get; }

            public StringClass(string stringProperty)
            {
                StringProperty = stringProperty;
            }
        }

        public class Consumer1
        {
            public StringClass StringClass { get; }

            public Consumer1(StringClass stringClass)
            {
                StringClass = stringClass;
            }
        }

        public class Consumer2
        {
            public StringClass StringClass { get; }

            public Consumer2(StringClass stringClass)
            {
                StringClass = stringClass;
            }
        }
    }
}