using NUnit.Framework;
using SexyInject.Emit;
using SexyInject.Emit.Signatures;

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

        [Test]
        public void AEFiveSeven()
        {
            var result = SignatureDecompressor.Decompress(0xAE, 0x57);
            Assert.AreEqual(0x2E57, result[0]);
        }

        [Test]
        public void BFFF()
        {
            var result = SignatureDecompressor.Decompress(0xBF, 0xFF);
            Assert.AreEqual(0x3FFF, result[0]);
        }

        [Test]
        public void CZeroZeroZeroFourZeroZeroZero()
        {
            var result = SignatureDecompressor.Decompress(0xC0, 0x00, 0x40, 0x00);
            Assert.AreEqual(0x4000, result[0]);
        }

        [Test]
        public void DFFFFFFF()
        {
            var result = SignatureDecompressor.Decompress(0xDF, 0xFF, 0xFF, 0xFF);
            Assert.AreEqual(0x1FFFFFFF, result[0]);
        }
    }
}