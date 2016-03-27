using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineI8Instruction : ILInstruction
    {
        internal InlineI8Instruction(MethodBase containingMethod, int offset, OpCode opCode, long value)
            : base(containingMethod, offset, opCode)
        {
            Int64 = value;
        }

        public long Int64 { get; }

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitInlineI8Instruction(this);
        }
    }
}