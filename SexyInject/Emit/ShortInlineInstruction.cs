using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ShortInlineIInstruction : ILInstruction
    {
        internal ShortInlineIInstruction(int offset, OpCode opCode, byte value) : base(offset, opCode)
        {
            Byte = value;
        }

        public byte Byte { get; }

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitShortInlineIInstruction(this);
        }
    }
}