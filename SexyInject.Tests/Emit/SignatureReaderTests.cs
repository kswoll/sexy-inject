using System;
using System.Linq;
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

        [Test]
        public void ReadMethodDefinitionReturnVoid()
        {
            var signature = GetSignature(typeof(MethodClass).GetMethod(nameof(MethodClass.VoidMethod)));
            var reader = new SignatureReader(signature);
            var method = reader.ReadMethodDef();
            Assert.AreEqual(SignatureTypeKind.Void, method.ReturnType.Type.TypeKind);
        }

        [Test]
        public void ReadMethodDefinitionReturnInt()
        {
            var signature = GetSignature(typeof(MethodClass).GetMethod(nameof(MethodClass.IntMethod)));
            var reader = new SignatureReader(signature);
            var method = reader.ReadMethodDef();
            Assert.AreEqual(SignatureTypeKind.I4, method.ReturnType.Type.TypeKind);
        }

        [Test]
        public void ReadMethodDefinitionReturnIntIntParameter()
        {
            var signature = GetSignature(typeof(MethodClass).GetMethod(nameof(MethodClass.IntMethodIntParameter)));
            var reader = new SignatureReader(signature);
            var method = reader.ReadMethodDef();
            Assert.AreEqual(SignatureTypeKind.I4, method.ReturnType.Type.TypeKind);
            Assert.AreEqual(SignatureTypeKind.I4, method.Parameters.Single().Type.TypeKind);
        }

        [Test]
        public void ReadMethodDefinitionGeneric()
        {
            var signature = GetSignature(typeof(MethodClass).GetMethod(nameof(MethodClass.GenericMethod)));
            var reader = new SignatureReader(signature);
            var method = reader.ReadMethodDef();
            Assert.AreEqual(SignatureTypeKind.MVar, method.ReturnType.Type.TypeKind);
            Assert.AreEqual(0, ((GenericMethodParameter)method.ReturnType.Type).Number);
            Assert.AreEqual(SignatureTypeKind.MVar, method.Parameters.Single().Type.TypeKind);
            Assert.AreEqual(0, ((GenericMethodParameter)method.Parameters.Single().Type).Number);
            Assert.AreEqual(1, method.GenericParameterCount);
        }

        private byte[] GetSignature(MemberInfo member)
        {
            return member.DeclaringType.Module.ResolveSignature(member.MetadataToken);
        }
    }
}