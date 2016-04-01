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

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitInlineIInstruction(this);
        public override void Emit(ILGenerator il) => il.Emit(OpCode, Int32);
        public override string ToString() => $"{base.ToString()} {Int32}";
    }
}