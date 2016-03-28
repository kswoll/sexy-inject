using System.Collections.Generic;
using System.Linq;

namespace SexyInject.Emit.Signatures
{
    public class SignatureProperty
    {
        public IReadOnlyList<SignatureCustomModifier> CustomModifiers { get; }
        public bool HasThis { get; } 
        public SignatureType Type { get; }
        public IReadOnlyList<SignatureParam> Parameters { get; }

        public SignatureProperty(IEnumerable<SignatureCustomModifier> customModifiers, bool hasThis, SignatureType type, IEnumerable<SignatureParam> parameters)
        {
            CustomModifiers = customModifiers.ToList();
            HasThis = hasThis;
            Type = type;
            Parameters = parameters.ToList();
        }
    }
}