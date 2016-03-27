using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineRInstruction : ILInstruction
    {
        internal InlineRInstruction(MethodBase containingMethod, int offset, OpCode opCode, double value) : base(containingMethod, offset, opCode)
        {
            Double = value;
        }

        public double Double { get; }

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitInlineRInstruction(this);
        }
    }
}