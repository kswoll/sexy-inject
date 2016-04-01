using System.Collections.Generic;
using System.Linq;

namespace SexyInject.Emit
{
    public class ILInterpreter
    {
        public IEnumerable<ILInstructionPacket> Interpret(IEnumerable<ILInstruction> instructions)
        {
            var stack = new Stack<ILInstructionPacket>();
            foreach (var instruction in instructions)
            {
                var pop = Enumerable.Range(0, instruction.GetPopCount()).Select(x => stack.Pop()).ToArray();
                
            }
        }
    }
}