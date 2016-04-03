using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SexyInject.Emit
{
    public class ILInstructionPacket
    {
        public ILInstructionPacket Parent { get; private set; }
        public ILInstruction Instruction { get; } 
        public IReadOnlyList<ILInstructionPacket> ChildInstructions { get; }

        private static readonly IReadOnlyList<ILInstructionPacket> instructions = new List<ILInstructionPacket>();

        public ILInstructionPacket(ILInstruction instruction)
        {
            Instruction = instruction;
            ChildInstructions = instructions;
        }

        public ILInstructionPacket(ILInstruction instruction, IEnumerable<ILInstructionPacket> childInstructions)
        {
            Instruction = instruction;
            ChildInstructions = childInstructions.ToList();
            foreach (var child in ChildInstructions)
                child.Parent = this;
        }

        public IEnumerable<ILInstruction> UnwrapInstructions()
        {
            foreach (var child in ChildInstructions)
            {
                foreach (var instruction in child.UnwrapInstructions())
                    yield return instruction;
            }
            yield return Instruction;
        }

        public IEnumerable<ILInstructionPacket> UnwrapPackets()
        {
            foreach (var child in ChildInstructions)
            {
                foreach (var packet in child.UnwrapPackets())
                    yield return packet;
            }
            yield return this;
        }

        public override string ToString()
        {
            return $"{Instruction}";
        }
    }
}