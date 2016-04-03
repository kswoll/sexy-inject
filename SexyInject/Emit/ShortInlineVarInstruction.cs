using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ShortInlineVarInstruction : ILInstruction
    {
        internal ShortInlineVarInstruction(MethodBase containingMethod, int offset, OpCode opCode, byte ordinal) : base(containingMethod, offset, opCode)
        {
            Ordinal = ordinal;
        }

        public byte Ordinal { get; }

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitShortInlineVarInstruction(this);
        public override void Emit(ILGenerator il) => il.Emit(OpCode, Ordinal);

        public override string ToString()
        {
            return $"{base.ToString()} {Ordinal}";
        }
    }
}