namespace SexyInject.Emit.Signatures
{
    public class SignatureCustomModifier
    {
        public bool IsRequired { get; } 
        public SignatureClass Class { get; }

        public SignatureCustomModifier(bool isRequired, SignatureClass @class)
        {
            IsRequired = isRequired;
            Class = @class;
        }
    }
}