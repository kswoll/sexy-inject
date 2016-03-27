using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineBrTargetInstruction : ILInstruction
    {
        private readonly int delta;

        internal InlineBrTargetInstruction(MethodBase containingMethod, int offset, OpCode opCode, int delta) : base(containingMethod, offset, opCode)
        {
            this.delta = delta;
        }

        public int Delta => delta;
        public int TargetOffset => offset + delta + 1 + 4;

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitInlineBrTargetInstruction(this);
        }
    }
}