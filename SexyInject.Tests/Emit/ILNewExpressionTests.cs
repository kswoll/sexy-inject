using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SexyInject.Emit;

namespace SexyInject.Tests.Emit
{
    [TestFixture]
    public class ILNewExpressionTests
    {
        [Test]
        public void NoArgumentConstructor()
        {
            var newExpression = Construct(_ => new NoArgumentConstructorClass());
            Assert.AreEqual(typeof(NoArgumentConstructorClass).GetConstructor(new Type[0]), newExpression.ConstructorInstruction.Method);
        }

        class NoArgumentConstructorClass {}

        ILNewExpression Construct<T>(Func<ResolveContext, T> constructor)
        {
            var instructions = new ILReader(constructor.GetMethodInfo()).ToArray();
            return ILNewExpression.Decompile(instructions);
        }
    }
}