using System.Collections.Generic;
using System.Linq;

namespace SexyInject.Emit.Signatures
{
    public class SignatureMethod
    {
        public SignatureFlags Flags { get; }
        public IReadOnlyList<SignatureParam> Parameters { get; }
        public SignatureParam ReturnType { get; }
        public int GenericParameterCount { get; }

        public SignatureMethod(SignatureFlags flags, IEnumerable<SignatureParam> parameters, SignatureParam returnType, int genericParameterCount)
        {
            Flags = flags;
            Parameters = parameters.ToList();
            ReturnType = returnType;
            GenericParameterCount = genericParameterCount;
        }
    }
}