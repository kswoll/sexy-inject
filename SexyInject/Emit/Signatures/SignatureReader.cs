using System;
using System.Collections.Generic;

namespace SexyInject.Emit.Signatures
{
    public class SignatureReader
    {
        private readonly byte[] data;
        private int position;
        private int lastPosition;

        public SignatureReader(byte[] data)
        {
            this.data = data;
        }

        public SignatureFlags ReadFlags()
        {
            return (SignatureFlags)ReadUnsignedInt();
        }

        private void Back()
        {
            if (lastPosition == -1)
                throw new InvalidOperationException("Cannot back up more than once.");
            position = lastPosition;
            lastPosition = -1;
        }

        public int ReadUnsignedInt()
        {
            lastPosition = position;
            var b1 = data[position++];
            if (!IsBitSet(b1, 7))
            {
                return b1;
            }
            else
            {
                var b2 = data[position++];
                b1 = (byte)UnsetBit(b1, 7);
                var twoByteValue = ((b1 << 8) + b2);
                if (!IsBitSet(twoByteValue, 14))
                {
                    return twoByteValue;
                }
                else
                {
                    twoByteValue = (int)UnsetBit(twoByteValue, 14);
                    var b3 = data[position++];
                    var b4 = data[position++];
                    var fourByteValue = (twoByteValue << 16) + (b3 << 8) + b4;
                    return fourByteValue;
                }
            }
        }

        public SignatureTypeKind ReadElementType()
        {
            return (SignatureTypeKind)ReadUnsignedInt();
        }

        public SignatureType ReadType()
        {
            var elementType = ReadElementType();
            return new SignatureType(elementType);
        }

        public SignatureClass ReadTypeEncoded()
        {
            var value = ReadUnsignedInt();
            var type = (SignatureTypeTable)(value & (1 + (1 << 1)));
            var index = value >> 2;
            return new SignatureClass(type, index);
        }

        public SignatureField ReadField()
        {
            var flag = ReadFlags();
            if (flag != SignatureFlags.Field)
                throw new InvalidOperationException("Not on a field");
            var modifiers = ReadCustomModifiers();
            var type = ReadType();
            return new SignatureField(modifiers, type);
        }

        public SignatureProperty ReadProperty()
        {
            var flag = ReadFlags();
            if (flag != SignatureFlags.Property)
                throw new InvalidOperationException("Not on a property");
            var parameterCount = ReadUnsignedInt();
            var modifiers = ReadCustomModifiers();
            var type = ReadType();
            var parameters = ReadParams(parameterCount);
            return new SignatureProperty(modifiers, flag.HasFlag(SignatureFlags.HasThis), type, parameters);
        }

        public SignatureCustomModifier ReadCustomModifier()
        {
            var elementType = ReadElementType();
            if (!elementType.HasFlag(SignatureTypeKind.CmodOpt) && !elementType.HasFlag(SignatureTypeKind.CmodReqd))
            {
                Back();
                return null;
            }
            var type = ReadTypeEncoded();
            return new SignatureCustomModifier(elementType.HasFlag(SignatureTypeKind.CmodReqd), type);
        }

        public IEnumerable<SignatureCustomModifier> ReadCustomModifiers()
        {
            var result = new List<SignatureCustomModifier>();
            for (var customModifier = ReadCustomModifier(); customModifier != null; customModifier = ReadCustomModifier())
            {
                result.Add(customModifier);
            }
            return result;
        }

        public SignatureParam ReadParam()
        {
            var customModifiers = ReadCustomModifiers();
            var elementType = ReadElementType();
            var type = elementType == SignatureTypeKind.ByRef ? ReadType() : null;
            return new SignatureParam(customModifiers, type == null, type);
        }

        public IEnumerable<SignatureParam> ReadParams(int count)
        {
            for (var i = 0; i < count; i++)
                yield return ReadParam();
        }

        private static bool IsBitSet(long b, int position)
        {
            return (b & (1 << position)) != 0;
        }

        private static long UnsetBit(long value, int position)
        {
            int mask = 1 << position;
            return value & ~mask;
        }
    }
}