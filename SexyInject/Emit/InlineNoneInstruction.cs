using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineNoneInstruction : ILInstruction
    {
        internal InlineNoneInstruction(int offset, OpCode opCode) : base(offset, opCode)
        {
        }

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitInlineNoneInstruction(this);
        }
    }
}