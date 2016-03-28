namespace SexyInject.Emit.Signatures
{
    public class SignatureClass
    {
        public SignatureTypeTable Table { get; } 
        public int Index { get; }

        public SignatureClass(SignatureTypeTable table, int index)
        {
            Table = table;
            Index = index;
        }
    }
}