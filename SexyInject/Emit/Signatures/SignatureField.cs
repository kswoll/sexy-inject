using System.Collections.Generic;
using System.Linq;

namespace SexyInject.Emit.Signatures
{
    public class SignatureField
    {
        public IReadOnlyList<SignatureCustomModifier> CustomModifiers { get; }
        public SignatureType Type { get; }

        public SignatureField(IEnumerable<SignatureCustomModifier> customModifiers, SignatureType type)
        {
            CustomModifiers = customModifiers.ToList();
            Type = type;
        }
    }
}