using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ShortInlineRInstruction : ILInstruction
    {
        internal ShortInlineRInstruction(MethodBase containingMethod, int offset, OpCode opCode, float value) : base(containingMethod, offset, opCode)
        {
            Single = value;
        }

        public float Single { get; }

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitShortInlineRInstruction(this);
        public override void Emit(ILGenerator il) => il.Emit(OpCode, Single);
    }
}