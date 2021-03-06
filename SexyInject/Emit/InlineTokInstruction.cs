﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class InlineTokInstruction : ILInstruction
    {
        private readonly ITokenResolver resolver;
        private MemberInfo member;

        internal InlineTokInstruction(MethodBase containingMethod, int offset, OpCode opCode, int token, ITokenResolver resolver) : base(containingMethod, offset, opCode)
        {
            this.resolver = resolver;
            Token = token;
        }

        public MemberInfo Member => member ?? (member = resolver.AsMember(Token));
        public int Token { get; }

        public override void Accept(ILInstructionVisitor vistor) => vistor.VisitInlineTokInstruction(this);

        public override void Emit(ILGenerator il)
        {
            var member = Member;
            if (member is Type)
                il.Emit(OpCode, (Type)member);
            else if (member is FieldInfo)
                il.Emit(OpCode, (FieldInfo)member);
            else
                il.Emit(OpCode, (MethodInfo)member);
        }
    }
}