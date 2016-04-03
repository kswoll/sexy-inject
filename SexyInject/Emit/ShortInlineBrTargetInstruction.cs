using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ShortInlineBrTargetInstruction : ILInstruction
    {
        private readonly sbyte delta;

        internal ShortInlineBrTargetInstruction(MethodBase containingMethod, int offset, OpCode opCode, sbyte delta) : base(containingMethod, offset, opCode)
        {
            this.delta = delta;
        }

        public sbyte Delta => delta;
        public int TargetOffset => offset + delta + 1 + 1;

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitShortInlineBrTargetInstruction(this);

        public override void Emit(ILGenerator il)
        {
            throw new NotImplementedException();
        }
    }
}