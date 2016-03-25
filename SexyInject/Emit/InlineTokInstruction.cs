using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineTokInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private MemberInfo member;

        internal InlineTokInstruction(int offset, OpCode opCode, int token, ITokenResolver resolver) : base(offset, opCode)
        {
            this.resolver = resolver;
            Token = token;
        }

        public MemberInfo Member => member ?? (member = resolver.AsMember(Token));
        public int Token { get; }

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineTokInstruction(this); }
    }
}