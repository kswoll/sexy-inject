namespace SexyInject.Emit.Signatures
{
    public class SignatureArrayType : SignatureType
    {
        public SignatureClass Class { get; }
        public SignatureArrayShape Shape { get; }

        public SignatureArrayType(SignatureTypeKind typeKind, SignatureClass @class, SignatureArrayShape shape) : base(typeKind)
        {
            Class = @class;
            Shape = shape;
        }
    }
}