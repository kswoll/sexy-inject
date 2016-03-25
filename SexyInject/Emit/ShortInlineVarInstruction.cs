using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ShortInlineVarInstruction : ILInstruction
    {
        internal ShortInlineVarInstruction(int offset, OpCode opCode, byte ordinal) : base(offset, opCode)
        {
            Ordinal = ordinal;
        }

        public byte Ordinal { get; }

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitShortInlineVarInstruction(this);
        }
    }
}