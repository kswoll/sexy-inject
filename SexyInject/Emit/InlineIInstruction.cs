using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineIInstruction : ILInstruction
    {
        internal InlineIInstruction(MethodBase containingMethod, int offset, OpCode opCode, int value) : base(containingMethod, offset, opCode)
        {
            Int32 = value;
        }

        public int Int32 { get; }

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitInlineIInstruction(this);
        }

        public override string ToString()
        {
            return $"{base.ToString()} {Int32}";
        }
    }
}