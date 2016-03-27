using System;
using System.Collections.Generic;
using System.Linq;

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

        public static ILNewExpression Decompile(ILInstruction[] instructions)
        {
            var arguments = new List<ILArgumentExpression>();
            for (var i = 0; i < instructions.Length; i++)
            {
                var instruction = instructions[i];
            }
            InlineMethodInstruction constructorInstruction = null;
//            int i = instructions.Length - 1;
/*
            for (; i >= 0 && constructorInstruction == null; i--)
            {
                constructorInstruction = instructions[i] as InlineMethodInstruction;
            }
            if (constructorInstruction == null || !constructorInstruction.Method.IsConstructor)
            {
                throw new ArgumentException("No invocation to a constructor", nameof(instructions));
            }
*/
            return new ILNewExpression(constructorInstruction, Enumerable.Empty<ILArgumentExpression>());
        }
    }
}