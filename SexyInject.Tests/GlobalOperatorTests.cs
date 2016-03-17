using NUnit.Framework;
using SexyInject.Tests.TestClasses;

namespace SexyInject.Tests
{
    [TestFixture]
    public class GlobalOperatorTests
    {
        [Test]
        public void GlobalHeadOperatorBeforeBinding()
        {
            var registry = new Registry();
            registry.AddGlobalHeadOperator(context => context.WhenResolved(o => ((IntClass)o).IntProperty++));
            registry.Bind<IntClass>(x => x.To().Cache(Cache.Singleton));
            registry.Get<IntClass>();
            var instance = registry.Get<IntClass>();
            Assert.AreEqual(1, instance.IntProperty);
        }

        [Test]
        public void GlobalTailOperatorBeforeBinding()
        {
            var registry = new Registry();
            registry.AddGlobalTailOperator(context => context.WhenResolved(o => ((IntClass)o).IntProperty++));
            registry.Bind<IntClass>(x => x.To().Cache(Cache.Singleton));
            registry.Get<IntClass>();
            var instance = registry.Get<IntClass>();
            Assert.AreEqual(2, instance.IntProperty);
        }

        [Test]
        public void GlobalHeadOperatorAfterBinding()
        {
            var registry = new Registry();
            registry.Bind<IntClass>(x => x.To().Cache(Cache.Singleton));
            registry.AddGlobalHeadOperator(context => context.WhenResolved(o => ((IntClass)o).IntProperty++));
            registry.Get<IntClass>();
            var instance = registry.Get<IntClass>();
            Assert.AreEqual(1, instance.IntProperty);
        }

        [Test]
        public void GlobalTailOperatorAfterBinding()
        {
            var registry = new Registry();
            registry.Bind<IntClass>(x => x.To().Cache(Cache.Singleton));
            registry.AddGlobalTailOperator(context => context.WhenResolved(o => ((IntClass)o).IntProperty++));
            registry.Get<IntClass>();
            var instance = registry.Get<IntClass>();
            Assert.AreEqual(2, instance.IntProperty);
        }

        public class IntClass
        {
            public int IntProperty { get; set; }
        }
    }
}