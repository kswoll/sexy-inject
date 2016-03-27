using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineVarInstruction : ILInstruction
    {
        internal InlineVarInstruction(MethodBase containingMethod, int offset, OpCode opCode, ushort ordinal) : base(containingMethod, offset, opCode)
        {
            Ordinal = ordinal;
        }

        public ushort Ordinal { get; }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineVarInstruction(this); }
    }
}