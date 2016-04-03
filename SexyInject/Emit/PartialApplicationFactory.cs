using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SexyInject.Emit
{
    public static class PartialApplicationFactory
    {
        private static readonly ConcurrentDictionary<MethodInfo, DynamicMethod> cache = new ConcurrentDictionary<MethodInfo, DynamicMethod>();

        public static Func<ResolveContext, T> CreateFactory<T>(Func<ResolveContext, T> factoryFunction)
        {
            var method = factoryFunction.GetMethodInfo();
            var dynamicMethod = cache.GetOrAdd(method, _ =>
            {
                if (Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute)))
                    throw new InvalidFactoryException("Factory functions cannot use async/await");

                var ilReader = new ILReader(method);
                var instructions = ilReader.ToArray();

                var instructionsByLocalVariable = new Dictionary<int, List<ILInstructionPacket>>();

                var stack = new Stack<ILInstructionPacket>();
                foreach (var instruction in instructions)
                {
                    if (instruction.OpCode == OpCodes.Switch || instruction.OpCode == OpCodes.Calli)
                        throw new InvalidFactoryException($"Unsupported op code in factory function: {instruction.OpCode}");

                    var popCount = instruction.GetPopCount();
                    if (instruction.OpCode == OpCodes.Dup)
                        popCount = 0;
                    var popped = Enumerable.Range(0, popCount).Select(x => stack.Pop()).Reverse().ToArray();
                    var packet = new ILInstructionPacket(instruction, popped);
                    stack.Push(packet);
                }

                foreach (var packet in stack.SelectMany(x => x.UnwrapPackets()))
                {
                    int localIndex;
                    if (packet.Instruction.TryGetLocal(out localIndex))
                    {
                        List<ILInstructionPacket> localInstructions;
                        if (!instructionsByLocalVariable.TryGetValue(localIndex, out localInstructions))
                        {
                            localInstructions = new List<ILInstructionPacket>();
                            instructionsByLocalVariable[localIndex] = localInstructions;
                        }
                        localInstructions.Add(packet);
                    }
                
                }

                var returnInstruction = stack.Pop();
                if (returnInstruction.Instruction.OpCode != OpCodes.Ret)
                    throw new InvalidFactoryException("The last operation in a factory must return the instance.");

                var constructorInvocation = returnInstruction.ChildInstructions.Single();
                if (constructorInvocation.Instruction.OpCode != OpCodes.Newobj)
                    throw new InvalidFactoryException("The factory should be a simple expression that instantiates the desired type.");

                var injectedMethod = new DynamicMethod(method.Name, method.ReturnType, new[] { factoryFunction.Target.GetType() }.Concat(method.GetParameters().Select(x => x.ParameterType)).ToArray(), factoryFunction.Target.GetType());
                var il = injectedMethod.GetILGenerator();
                foreach (var local in method.GetMethodBody().LocalVariables)
                {
                    il.DeclareLocal(local.LocalType, local.IsPinned);
                }

                foreach (var prelude in stack.Reverse())
                {
                    foreach (var instruction in prelude.UnwrapInstructions())
                    {
                        instruction.Emit(il);
                    }
                }

                var constructorInstruction = (InlineMethodInstruction)constructorInvocation.Instruction;
                var constructor = (ConstructorInfo)constructorInstruction.Method;
                var arguments = constructorInvocation.ChildInstructions.ToArray();
                var parameters = constructor.GetParameters();
                for (var i = 0; i < parameters.Length; i++)
                {
                    var argument = arguments[i];
                    var parameter = parameters[i];
                    if (parameter.IsOptional && IsDefaultValue(argument, parameter.ParameterType, parameter.DefaultValue, instructionsByLocalVariable))
                    {
                        // Use the context to resolve the argument
                        il.Emit(OpCodes.Ldarg_1);
                        il.LoadType(parameter.ParameterType);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Newarr, typeof(object));
                        il.EmitCall(OpCodes.Call, ResolveContext.ResolveMethod, null);
                        if (parameter.ParameterType.IsValueType || parameter.ParameterType.IsGenericParameter)
                        {
                            il.Emit(OpCodes.Unbox_Any, parameter.ParameterType);
                        }
                    }
                    else
                    {
                        foreach (var instruction in argument.UnwrapInstructions())
                            instruction.Emit(il);                    
                    }
                }
                constructorInstruction.Emit(il);
                returnInstruction.Instruction.Emit(il);

                return injectedMethod;
            });
            return (Func<ResolveContext, T>)dynamicMethod.CreateDelegate(typeof(Func<ResolveContext, T>), factoryFunction.Target);
        }

        private static bool IsDefaultValue(ILInstructionPacket packet, Type type, object defaultValue, Dictionary<int, List<ILInstructionPacket>> locals)
        {
            var instruction = packet.Instruction;
            if (packet.Instruction.OpCode == OpCodes.Conv_I8)
                instruction = packet.ChildInstructions.Single().Instruction;
            if (defaultValue is int || defaultValue is uint || defaultValue is long || defaultValue is ulong || defaultValue is short || defaultValue is ushort ||
                defaultValue is byte || defaultValue is sbyte)
            {
                var value = (instruction as InlineIInstruction)?.Int32 ?? (instruction as ShortInlineIInstruction)?.Byte;
                if (defaultValue.Equals((-1).ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_M1 || value == -1;
                else if (defaultValue.Equals(0.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_0 || value == 0;
                else if (defaultValue.Equals(1.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_1 || value == 1;
                else if (defaultValue.Equals(2.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_2 || value == 2;
                else if (defaultValue.Equals(3.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_3 || value == 3;
                else if (defaultValue.Equals(4.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_4 || value == 4;
                else if (defaultValue.Equals(5.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_5 || value == 5;
                else if (defaultValue.Equals(6.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_6 || value == 6;
                else if (defaultValue.Equals(7.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_7 || value == 7;
                else if (defaultValue.Equals(8.ToLiteral(type)))
                    return instruction.OpCode == OpCodes.Ldc_I4_8 || value == 8;
                else
                    return value.ToLiteral(type).Equals(defaultValue);
            }
            else if (defaultValue is float || defaultValue is double)
            {
                var value = (instruction as InlineRInstruction)?.Double ?? (instruction as ShortInlineRInstruction)?.Single;
                return value.ToLiteral(type).Equals(defaultValue);
            }
            else if (defaultValue is decimal)
            {
                if (packet.ChildInstructions.Count == 5)
                {
                    var lo = packet.ChildInstructions[0].Instruction.GetIntValue();
                    var mid = packet.ChildInstructions[1].Instruction.GetIntValue();
                    var hi = packet.ChildInstructions[2].Instruction.GetIntValue();
                    var isNegative = packet.ChildInstructions[3].Instruction.GetIntValue();
                    var scale = packet.ChildInstructions[4].Instruction.GetIntValue();
                    if (lo != null && mid != null && hi != null && isNegative != null && scale != null)
                    {
                        var value = new Decimal(lo.Value, mid.Value, hi.Value, isNegative.Value == 1, (byte)scale.Value);
                        return value.Equals(defaultValue);
                    }
                }

                return false;
            }
            else if (defaultValue is bool)
            {
                if (defaultValue.Equals(false))
                    return instruction.OpCode == OpCodes.Ldc_I4_0;
                else 
                    return instruction.OpCode == OpCodes.Ldc_I4_1;
            }
            else if (type.IsValueType)
            {
                int localIndex;
                if (instruction.TryGetLocal(out localIndex))
                {
                    List<ILInstructionPacket> packets;
                    if (locals.TryGetValue(localIndex, out packets))
                    {
                        if (packets.Count == 2)
                            return true;
                    }
                }
                return false;
            }
            else
            {
                return instruction.OpCode == OpCodes.Ldnull;
            }
        }

        private static int? GetIntValue(this ILInstruction instruction)
        {
            var op = instruction.OpCode;
            if (op == OpCodes.Ldc_I4)
                return (instruction as InlineIInstruction)?.Int32 ?? ((ShortInlineIInstruction)instruction).Byte;
            else if (op == OpCodes.Ldc_I4_0)
                return 0;
            else if (op == OpCodes.Ldc_I4_1)
                return 1;
            else if (op == OpCodes.Ldc_I4_2)
                return 2;
            else if (op == OpCodes.Ldc_I4_3)
                return 3;
            else if (op == OpCodes.Ldc_I4_4)
                return 4;
            else if (op == OpCodes.Ldc_I4_5)
                return 5;
            else if (op == OpCodes.Ldc_I4_6)
                return 6;
            else if (op == OpCodes.Ldc_I4_7)
                return 7;
            else if (op == OpCodes.Ldc_I4_8)
                return 8;
            else if (op == OpCodes.Ldc_I4_M1)
                return -1;
            else if (op == OpCodes.Ldc_I4_0)
                return 0;
            else
                return null;
        }

        private static object ToLiteral<T>(this T value, Type type)
        {
            return Convert.ChangeType(value, type);
        }
    }
}