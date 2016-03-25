using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public abstract class ILInstruction
    {
        protected int offset;
        protected OpCode opCode;

        internal ILInstruction(int offset, OpCode opCode)
        {
            this.offset = offset;
            this.opCode = opCode;
        }

        public int Offset => offset;
        public OpCode OpCode => opCode;

        public abstract void Accept(ILInstructionVisitor vistor);
    }
}