using System.Collections.Generic;

namespace SexyInject.Emit
{
    public class ILInterpreter
    {
        public IEnumerable<ILInstructionPacket> Interpret(IEnumerable<ILInstruction> instructions)
        {
            var stack = new Stack<ILInstructionPacket>();
            return null;
            foreach (var instruction in instructions)
            {
            }
        }
    }
}