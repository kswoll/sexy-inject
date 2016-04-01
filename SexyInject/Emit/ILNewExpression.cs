using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ILNewExpression
    {
        public InlineMethodInstruction ConstructorInstruction { get; }
        public List<ILArgumentExpression> Arguments { get; }

        public ILNewExpression(InlineMethodInstruction constructorInstruction, IEnumerable<ILArgumentExpression> arguments)
        {
            ConstructorInstruction = constructorInstruction;
            Arguments = arguments.ToList();
        }

        public static DynamicMethod InjectConstructorCall(MethodInfo method, ILInstruction[] instructions)
        {
            InlineMethodInstruction constructorInstruction = null;
            int i = instructions.Length - 1;
            for (; i >= 0 && constructorInstruction == null; i--)
            {
                constructorInstruction = instructions[i] as InlineMethodInstruction;
            }
            if (constructorInstruction == null || !constructorInstruction.Method.IsConstructor)
            {
                throw new ArgumentException("No invocation to a constructor", nameof(instructions));
            }
            var injectedMethod = new DynamicMethod(method.Name, method.ReturnType, method.GetParameters().Select(x => x.ParameterType).ToArray(), method.DeclaringType);
            var il = injectedMethod.GetILGenerator();
            for (int j = 0; j < i; j++)
            {
                var instruction = instructions[j];
                il.Emit();
            }
        }
    }
}