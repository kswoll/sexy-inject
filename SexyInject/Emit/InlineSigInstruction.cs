using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineSigInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private readonly int token;
        private byte[] signature;

        internal InlineSigInstruction(MethodBase containingMethod, int offset, OpCode opCode, int token, ITokenResolver resolver) : base(containingMethod, offset, opCode)
        {
            this.resolver = resolver;
            this.token = token;
        }

        public byte[] Signature => signature ?? (signature = resolver.AsSignature(token));
        public int Token => token;

        public override void Accept(ILInstructionVisitor vistor) { vistor.VisitInlineSigInstruction(this); }
/*

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
*/
    }
}