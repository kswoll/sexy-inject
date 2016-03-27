using NUnit.Framework;
using SexyInject.Emit;

namespace SexyInject.Tests.Emit
{
    [TestFixture]
    public class SignatureDecompressorTests
    {
        [Test]
        public void ZeroThree()
        {
            var result = SignatureDecompressor.Decompress(0x03);
            Assert.AreEqual(3, result[0]);
        }

        [Test]
        public void SevenF()
        {
            var result = SignatureDecompressor.Decompress(0x7F);
            Assert.AreEqual(0x7F, result[0]);
        }

        [Test]
        public void EightZeroEightZero()
        {
            var result = SignatureDecompressor.Decompress(0x80, 0x80);
            Assert.AreEqual(0x80, result[0]);
        }
    }
}