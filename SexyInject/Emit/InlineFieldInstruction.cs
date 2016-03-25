using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineFieldInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private readonly int token;
        private FieldInfo field;

        internal InlineFieldInstruction(ITokenResolver resolver, int offset, OpCode opCode, int token) : base(offset, opCode)
        {
            this.resolver = resolver;
            this.token = token;
        }

        public FieldInfo Field => field ?? (field = resolver.AsField(token));
        public int Token => token;

        public override void Accept(ILInstructionVisitor vistor)
        {
            vistor.VisitInlineFieldInstruction(this);
        }
    }
}