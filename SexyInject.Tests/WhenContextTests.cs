using NUnit.Framework;
using SexyInject.Tests.TestClasses;

namespace SexyInject.Tests
{
    [TestFixture]
    public class WhenContextTests
    {
        [Test]
        public void BinderProperty()
        {
            var registry = new Registry();
            var binder = new Binder<SimpleClass>(registry);
            var whenContext = binder.When(x => true);
            Assert.AreEqual(binder, ((WhenContext)whenContext).Binder);
        }
    }
}