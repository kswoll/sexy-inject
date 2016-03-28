using System.Collections.Generic;
using System.Linq;

namespace SexyInject.Emit.Signatures
{
    public class SignatureParam
    {
        public IReadOnlyList<SignatureCustomModifier> CustomModifiers { get; }
        public bool IsTypedByRef { get; }
        public SignatureType Type { get; }

        public SignatureParam(IEnumerable<SignatureCustomModifier> customModifiers, bool isTypedByRef, SignatureType type)
        {
            CustomModifiers = customModifiers.ToList();
            IsTypedByRef = isTypedByRef;
            Type = type;
        }
    }
}