using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SexyInject.Tests
{
    [TestFixture]
    public class RecordTest
    {
        [Test]
        public void ConstructorTest()
        {
            var registry = new Registry();
            registry.Bind<TestClass>();
            registry.RegisterPartialApplicationPattern();
            registry.Bind<X.Focus>();
            registry.Bind<X>();
            registry.Bind<string>();
            registry.Get<TestClass>();
        }

        public class TestClass
        {
            public TestClass(Constructor<X> xFactory)
            {
                var x = xFactory(_ => new X(4));
            }
        }

        public class X
        {
            public X(int some, Focus focus = null)
            {
            }

            public record Focus(string Uid);
        }
    }
}
