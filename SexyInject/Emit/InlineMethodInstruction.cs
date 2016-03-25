using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineMethodInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private readonly int token;
        private MethodBase method;

        internal InlineMethodInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver) : base(offset, opCode)
        {
            this.resolver = resolver;
            this.token = token;
        }

        public MethodBase Method => method ?? (method = resolver.AsMethod(token));
        public int Token => token;

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitInlineMethodInstruction(this);
        }
    }
}