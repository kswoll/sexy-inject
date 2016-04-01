using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public abstract class ILInstruction
    {
        protected readonly MethodBase containingMethod;
        protected readonly int offset;
        protected readonly OpCode opCode;

        internal ILInstruction(MethodBase containingMethod, int offset, OpCode opCode)
        {
            this.containingMethod = containingMethod;
            this.offset = offset;
            this.opCode = opCode;
        }

        public int Offset => offset;
        public OpCode OpCode => opCode;

        public abstract void Accept(ILInstructionVisitor vistor);
        public abstract void Emit(ILGenerator generator);

        public override string ToString()
        {
            return $"0x{offset.ToString("X").PadLeft(4, '0')} {opCode}";
        }

        public virtual int GetPopCount()
        {
            switch (opCode.StackBehaviourPop)
            {
                case StackBehaviour.Pop0:
                    return 0;
                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                    return 1;
                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                    return 2;
                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_pop1:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return 3;
                default:
                    throw new Exception($"Subclass should override GetPopCount() to provide correct value for the number of items popped off the stack for OpCode {OpCode}.");
            }
        }

        public virtual int GetPushCount()
        {
            switch (opCode.StackBehaviourPush)
            {
                case StackBehaviour.Push0:
                    return 0;
                case StackBehaviour.Push1:
                case StackBehaviour.Pushi:
                case StackBehaviour.Pushi8:
                case StackBehaviour.Pushr4:
                case StackBehaviour.Pushr8:
                case StackBehaviour.Pushref:
                    return 1;
                case StackBehaviour.Push1_push1:
                    return 1;
                default:
                    throw new Exception($"Subclass should override GetPushCount() to provide correct value for the number of items pushed onto the stack for OpCode {OpCode}.");
            }
        }
    }
}