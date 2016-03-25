using System;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineTypeInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private readonly int token;
        private Type type;

        internal InlineTypeInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver) : base(offset, opCode)
        {
            this.resolver = resolver;
            this.token = token;
        }

        public Type Type => type ?? (type = resolver.AsType(token));
        public Int32 Token => token;

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineTypeInstruction(this); }
    }
}