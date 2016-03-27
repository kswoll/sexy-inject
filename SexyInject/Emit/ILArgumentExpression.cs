using System.Collections.Generic;

namespace SexyInject.Emit
{
    public class ILArgumentExpression
    {
        public IReadOnlyList<InlineIInstruction> Instructions { get; }

        public ILArgumentExpression(IReadOnlyList<InlineIInstruction> instructions)
        {
            Instructions = instructions;
        }
    }
}