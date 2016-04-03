using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineTypeInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private readonly int token;
        private Type type;

        internal InlineTypeInstruction(MethodBase containingMethod, int offset, OpCode opCode, int token, ITokenResolver resolver) : base(containingMethod, offset, opCode)
        {
            this.resolver = resolver;
            this.token = token;
        }

        public Type Type => type ?? (type = resolver.AsType(token));
        public int Token => token;

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitInlineTypeInstruction(this);
        public override void Emit(ILGenerator il) => il.Emit(OpCode, Type);

        public override string ToString()
        {
            return $"{base.ToString()} {Type.FullName}";
        }
    }
}