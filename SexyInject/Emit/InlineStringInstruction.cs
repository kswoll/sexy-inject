using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineStringInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private string @string;

        internal InlineStringInstruction(MethodBase containingMethod, int offset, OpCode opCode, int token, ITokenResolver resolver) : base(containingMethod, offset, opCode)
        {
            this.resolver = resolver;
            Token = token;
        }

        public string String => @string ?? (@string = resolver.AsString(Token));
        public int Token { get; }

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitInlineStringInstruction(this);
        public override void Emit(ILGenerator il) => il.Emit(OpCode, String);

        public override string ToString()
        {
            return $"{base.ToString()} {String}";
        }
    }
}