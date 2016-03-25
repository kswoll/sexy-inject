using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ShortInlineBrTargetInstruction : ILInstruction
    {
        private readonly sbyte delta;

        internal ShortInlineBrTargetInstruction(int offset, OpCode opCode, sbyte delta) : base(offset, opCode)
        {
            this.delta = delta;
        }

        public sbyte Delta => delta;
        public int TargetOffset => offset + delta + 1 + 1;

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitShortInlineBrTargetInstruction(this);
        }
    }
}