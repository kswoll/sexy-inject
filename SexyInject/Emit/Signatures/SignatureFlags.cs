namespace SexyInject.Emit.Signatures
{
    public enum SignatureFlags : byte
    {
        HasThis = 0x20,
        ExplicitThis = 0x40,
        Vararg = 0x5,
        Sentinel = 0x41,
        C = 0x1,
        StdCall = 0x2,
        ThisCall = 0x3,
        FastCall = 0x4,
        Field = 0x6,
        LocalSig = 0x7,
        Property = 0x8,
        ByRef = 0x10,
        TypedByRef = 0x16
    }
}