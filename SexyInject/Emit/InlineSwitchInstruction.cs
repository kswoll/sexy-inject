using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineSwitchInstruction : ILInstruction
    {
        private readonly int[] deltas;
        private int[] targetOffsets;

        internal InlineSwitchInstruction(MethodBase containingMethod, int offset, OpCode opCode, int[] deltas) : base(containingMethod, offset, opCode)
        {
            this.deltas = deltas;
        }

        public int[] Deltas => (int[])deltas.Clone();

        public int[] TargetOffsets
        {
            get
            {
                if (targetOffsets == null)
                {
                    var cases = deltas.Length;
                    var itself = 1 + 4 + 4 * cases;
                    targetOffsets = new int[cases];
                    for (var i = 0; i < cases; i++)
                        targetOffsets[i] = offset + deltas[i] + itself;
                }
                return targetOffsets;
            }
        }

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitInlineSwitchInstruction(this);
        public override void Emit(ILGenerator il) => il.Emit(OpCode, Deltas);
    }
}