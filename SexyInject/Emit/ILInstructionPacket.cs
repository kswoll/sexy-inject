using System.Collections.Generic;
using System.Linq;

namespace SexyInject.Emit
{
    public class ILInstructionPacket
    {
        public ILInstruction Instruction { get; } 
        public IReadOnlyList<ILInstruction> ChildInstructions { get; }

        private static readonly IReadOnlyList<ILInstruction> instructions = new List<ILInstruction>();

        public ILInstructionPacket(ILInstruction instruction)
        {
            Instruction = instruction;
            ChildInstructions = instructions;
        }

        public ILInstructionPacket(ILInstruction instruction, IEnumerable<ILInstruction> childInstructions)
        {
            Instruction = instruction;
            ChildInstructions = childInstructions.ToList();
        }
    }
}