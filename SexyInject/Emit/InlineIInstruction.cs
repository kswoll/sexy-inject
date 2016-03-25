using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineIInstruction : ILInstruction
    {
        internal InlineIInstruction(int offset, OpCode opCode, int value) : base(offset, opCode)
        {
            Int32 = value;
        }

        public int Int32 { get; }

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitInlineIInstruction(this);
        }
    }
}