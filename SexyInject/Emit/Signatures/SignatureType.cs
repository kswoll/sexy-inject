namespace SexyInject.Emit.Signatures
{
    public class SignatureType
    {
        public SignatureTypeKind TypeKind { get; }

        public SignatureType(SignatureTypeKind typeKind)
        {
            TypeKind = typeKind;
        }
    }
}