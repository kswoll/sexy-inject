using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    internal static class EmitExtensions
    {
        private static readonly MethodInfo getTypeFromRuntimeHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");

        public static void LoadType(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Ldtoken, type);
            il.Emit(OpCodes.Call, getTypeFromRuntimeHandleMethod);
        }

        public static bool TryGetLocal(this ILInstruction instruction, out int index)
        {
            var op = instruction.OpCode;
            if (op == OpCodes.Ldloc || op == OpCodes.Ldloca || op == OpCodes.Stloc)
            {
                index = ((InlineVarInstruction)instruction).Ordinal;
                return true;
            }
            else if (op == OpCodes.Ldloc_S || op == OpCodes.Ldloca_S || op == OpCodes.Stloc_S)
            {
                index = ((ShortInlineVarInstruction)instruction).Ordinal;
                return true;
            }
            else if (op == OpCodes.Ldloc_0 || op == OpCodes.Stloc_0)
            {
                index = 0;
                return true;
            }
            else if (op == OpCodes.Ldloc_1 || op == OpCodes.Stloc_1)
            {
                index = 1;
                return true;
            }
            else if (op == OpCodes.Ldloc_2 || op == OpCodes.Stloc_S)
            {
                index = 2;
                return true;
            }
            else if (op == OpCodes.Ldloc_3 || op == OpCodes.Stloc_3)
            {
                index = 3;
                return true;
            }
            else
            {
                index = -1;
                return false;
            }
        }

        public static bool IsLoadLocal(this ILInstruction instruction)
        {
            var op = instruction.OpCode;
            return op == OpCodes.Ldloc || op == OpCodes.Ldloc_S || op == OpCodes.Ldloc_0 || op == OpCodes.Ldloc_1 || 
                op == OpCodes.Ldloc_2 || op == OpCodes.Ldloc_3;
        }
    }
}