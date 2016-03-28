using System;
using System.Reflection;
using NUnit.Framework;
using SexyInject.Emit.Signatures;
using SexyInject.Tests.Emit.TestClasses;
using SexyInject.Tests.MetaData;

namespace SexyInject.Tests.Emit
{
    [TestFixture]
    public class SignatureReaderTests
    {
        [Test]
        public void ReadIntField()
        {
            var signature = GetSignature(typeof(FieldClass).GetField(nameof(FieldClass.IntField)));
            var reader = new SignatureReader(signature);
            var field = reader.ReadField();
            Assert.AreEqual(SignatureTypeKind.I4, field.Type.TypeKind);
        }

        [Test]
        public void ReadBoolField()
        {
            var signature = GetSignature(typeof(FieldClass).GetField(nameof(FieldClass.BoolField)));
            var reader = new SignatureReader(signature);
            var field = reader.ReadField();
            Assert.AreEqual(SignatureTypeKind.Boolean, field.Type.TypeKind);
        }

        [Test]
        public void ReadStringField()
        {
            var signature = GetSignature(typeof(FieldClass).GetField(nameof(FieldClass.StringField)));
            var reader = new SignatureReader(signature);
            var field = reader.ReadField();
            Assert.AreEqual(SignatureTypeKind.String, field.Type.TypeKind);
        }

        private byte[] GetSignature(MemberInfo member)
        {
            return member.DeclaringType.Module.ResolveSignature(member.MetadataToken);
        }
    }
}