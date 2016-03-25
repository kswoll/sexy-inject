using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ShortInlineRInstruction : ILInstruction
    {
        internal ShortInlineRInstruction(int offset, OpCode opCode, float value) : base(offset, opCode)
        {
            Single = value;
        }

        public float Single { get; }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitShortInlineRInstruction(this); }
    }
}