using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public sealed class ILReader : IEnumerable<ILInstruction>
    {
        private static readonly Type runtimeMethodInfoType = Type.GetType("System.Reflection.RuntimeMethodInfo");
        private static readonly Type runtimeConstructorInfoType = Type.GetType("System.Reflection.RuntimeConstructorInfo");
        
        private static readonly OpCode[] oneByteOpCodes;
        private static readonly OpCode[] twoByteOpCodes;

        static ILReader()
        {
            oneByteOpCodes = new OpCode[0x100];
            twoByteOpCodes = new OpCode[0x100];

            foreach (var fieldInfo in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OpCode opCode = (OpCode)fieldInfo.GetValue(null);
                var value = (ushort)opCode.Value;
                if (value < 0x100) {
                    oneByteOpCodes[value] = opCode;
                } else if ((value & 0xff00) == 0xfe00) {
                    twoByteOpCodes[value & 0xff] = opCode;
                }
            }
        }

        private int position;
        private readonly ITokenResolver resolver;
        private readonly byte[] byteArray;

        public ILReader(MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var type = method.GetType();
            if (type != runtimeMethodInfoType && type != runtimeConstructorInfoType)
            {
                throw new ArgumentException("method must be RuntimeMethodInfo or RuntimeConstructorInfo for this constructor.");
            }

            var ilProvider = new MethodBaseILProvider(method);
            resolver = new ModuleScopeTokenResolver(method);
            byteArray = ilProvider.GetByteArray();
        }

        public ILReader(IILProvider ilProvider, ITokenResolver tokenResolver)
        {
            if (ilProvider == null)
            {
                throw new ArgumentNullException(nameof(ilProvider));
            }

            resolver = tokenResolver;
            byteArray = ilProvider.GetByteArray();
            position = 0;
        }

        public IEnumerator<ILInstruction> GetEnumerator()
        {
            while (position < byteArray.Length)
                yield return Next();

            position = 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private ILInstruction Next()
        {
            var offset = position;
            OpCode opCode;
            int token;

            // read first 1 or 2 bytes as opCode
            var code = ReadByte();
            if (code != 0xFE)
            {
                opCode = oneByteOpCodes[code];
            }
            else
            {
                code = ReadByte();
                opCode = twoByteOpCodes[code];
            }

            switch (opCode.OperandType)
            {
                case OperandType.InlineNone:
                    return new InlineNoneInstruction(offset, opCode);

                //The operand is an 8-bit integer branch target.
                case OperandType.ShortInlineBrTarget:
                    var shortDelta = ReadSByte();
                    return new ShortInlineBrTargetInstruction(offset, opCode, shortDelta);

                //The operand is a 32-bit integer branch target.
                case OperandType.InlineBrTarget:
                    int delta = ReadInt32();
                    return new InlineBrTargetInstruction(offset, opCode, delta);

                //The operand is an 8-bit integer: 001F  ldc.i4.s, FE12  unaligned.
                case OperandType.ShortInlineI:
                    byte int8 = ReadByte();
                    return new ShortInlineIInstruction(offset, opCode, int8);

                //The operand is a 32-bit integer.
                case OperandType.InlineI:
                    int int32 = ReadInt32();
                    return new InlineIInstruction(offset, opCode, int32);

                //The operand is a 64-bit integer.
                case OperandType.InlineI8:
                    var int64 = ReadInt64();
                    return new InlineI8Instruction(offset, opCode, int64);

                //The operand is a 32-bit IEEE floating point number.
                case OperandType.ShortInlineR:
                    float float32 = ReadSingle();
                    return new ShortInlineRInstruction(offset, opCode, float32);

                //The operand is a 64-bit IEEE floating point number.
                case OperandType.InlineR:
                    var float64 = ReadDouble();
                    return new InlineRInstruction(offset, opCode, float64);

                //The operand is an 8-bit integer containing the ordinal of a local variable or an argument
                case OperandType.ShortInlineVar:
                    byte index8 = ReadByte();
                    return new ShortInlineVarInstruction(offset, opCode, index8);

                //The operand is 16-bit integer containing the ordinal of a local variable or an argument.
                case OperandType.InlineVar:
                    ushort index16 = ReadUInt16();
                    return new InlineVarInstruction(offset, opCode, index16);

                //The operand is a 32-bit metadata string token.
                case OperandType.InlineString:
                    token = ReadInt32();
                    return new InlineStringInstruction(offset, opCode, token, resolver);

                //The operand is a 32-bit metadata signature token.
                case OperandType.InlineSig:
                    token = ReadInt32();
                    return new InlineSigInstruction(offset, opCode, token, resolver);

                //The operand is a 32-bit metadata token.
                case OperandType.InlineMethod:
                    token = ReadInt32();
                    return new InlineMethodInstruction(offset, opCode, token, resolver);

                //The operand is a 32-bit metadata token.
                case OperandType.InlineField:
                    token = ReadInt32();
                    return new InlineFieldInstruction(resolver, offset, opCode, token);

                //The operand is a 32-bit metadata token.
                case OperandType.InlineType:
                    token = ReadInt32();
                    return new InlineTypeInstruction(offset, opCode, token, resolver);

                //The operand is a FieldRef, MethodRef, or TypeRef token.
                case OperandType.InlineTok:
                    token = ReadInt32();
                    return new InlineTokInstruction(offset, opCode, token, resolver);

                //The operand is the 32-bit integer argument to a switch instruction.
                case OperandType.InlineSwitch:
                    int cases = ReadInt32();
                    int[] deltas = new Int32[cases];
                    for (var i = 0; i < cases; i++)
                        deltas[i] = ReadInt32();
                    return new InlineSwitchInstruction(offset, opCode, deltas);

                default:
                    throw new BadImageFormatException("unexpected OperandType " + opCode.OperandType);
            }
        }

        public void Accept(ILInstructionVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));

            foreach (var instruction in this)
            {
                instruction.Accept(visitor);
            }
        }

        public byte ReadByte()
        {
            return byteArray[position++];
        }

        public sbyte ReadSByte() {
            return (sbyte)ReadByte();
        }

        public ushort ReadUInt16()
        {
            var pos = position;
            position += 2;
            return BitConverter.ToUInt16(byteArray, pos);
        }

        public uint ReadUInt32()
        {
            var pos = position;
            position += 4;
            return BitConverter.ToUInt32(byteArray, pos);
        }

        public ulong ReadUInt64()
        {
            var pos = position;
            position += 8;
            return BitConverter.ToUInt64(byteArray, pos);
        }

        public int ReadInt32()
        {
            var pos = position;
            position += 4;
            return BitConverter.ToInt32(byteArray, pos);
        }

        public long ReadInt64()
        {
            var pos = position;
            position += 8;
            return BitConverter.ToInt64(byteArray, pos);
        }

        public float ReadSingle()
        {
            var pos = position;
            position += 4;
            return BitConverter.ToSingle(byteArray, pos);
        }

        double ReadDouble()
        {
            var pos = position;
            position += 8;
            return BitConverter.ToDouble(byteArray, pos);
        }
    }
}