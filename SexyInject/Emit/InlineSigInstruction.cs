using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineSigInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private readonly int token;
        private byte[] signature;

        internal InlineSigInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver) : base(offset, opCode)
        {
            this.resolver = resolver;
            this.token = token;
        }

        public byte[] Signature => signature ?? (signature = resolver.AsSignature(token));
        public int Token => token;

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineSigInstruction(this); }
    }
}