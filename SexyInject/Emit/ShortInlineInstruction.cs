using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ShortInlineIInstruction : ILInstruction
    {
        internal ShortInlineIInstruction(MethodBase containingMethod, int offset, OpCode opCode, byte value) : base(containingMethod, offset, opCode)
        {
            Byte = value;
        }

        public byte Byte { get; }

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitShortInlineIInstruction(this);
        public override void Emit(ILGenerator il) => il.Emit(OpCode, Byte);
    }
}