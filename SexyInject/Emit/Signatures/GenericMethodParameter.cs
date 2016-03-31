namespace SexyInject.Emit.Signatures
{
    public class GenericMethodParameter : SignatureType
    {
        public int Number { get; }

        public GenericMethodParameter(SignatureTypeKind typeKind, int number) : base(typeKind)
        {
            Number = number;
        }
    }
}