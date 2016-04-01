using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineMethodInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private readonly int token;
        private MethodBase method;

        internal InlineMethodInstruction(MethodBase containingMethod, int offset, OpCode opCode, int token, ITokenResolver resolver) : base(containingMethod, offset, opCode)
        {
            this.resolver = resolver;
            this.token = token;
        }

        public MethodBase Method => method ?? (method = resolver.AsMethod(token));
        public int Token => token;

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitInlineMethodInstruction(this);

        public override void Emit(ILGenerator il)
        {
            if (Method is MethodInfo)
                il.Emit(OpCode, (MethodInfo)Method);
            else
                il.Emit(OpCode, (ConstructorInfo)Method);
        }

        public override string ToString() => $"{base.ToString()} {Method.DeclaringType.FullName}.{Method}";

        public override int GetPopCount()
        {
            if (OpCode == OpCodes.Call || OpCode == OpCodes.Callvirt || OpCode == OpCodes.Newobj)
                return Method.GetParameters().Length + (Method.IsStatic ? 0 : 1);
            else
                return base.GetPopCount();
        }

        public override int GetPushCount()
        {
            if (OpCode == OpCodes.Call || OpCode == OpCodes.Callvirt)
                return (Method as MethodInfo)?.ReturnType != typeof(void) ? 1 : 0;
            else
                return base.GetPushCount();
        }
    }
}