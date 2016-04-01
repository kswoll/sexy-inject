using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineNoneInstruction : ILInstruction
    {
        internal InlineNoneInstruction(MethodBase containingMethod, int offset, OpCode opCode) : base(containingMethod, offset, opCode)
        {
        }

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitInlineNoneInstruction(this);
        public override void Emit(ILGenerator il) => il.Emit(OpCode);

        public override int GetPopCount()
        {
            if (OpCode == OpCodes.Ret)
                return (containingMethod as MethodInfo)?.ReturnType != typeof(void) ? 1 : 0;
            else
                return base.GetPopCount();
        }
    }
}