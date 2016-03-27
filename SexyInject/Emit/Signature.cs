namespace SexyInject.Emit
{
    public class Signature
    {
        private const byte HasThis = 0x20;
        private const byte ExplicitThis = 0x40;
        private const byte Vararg = 0x5;
        private const byte Sentinel = 0x41;
        private const byte C = 0x1;
        private const byte StdCall = 0x2;
        private const byte ThisCall = 0x3;
        private const byte FastCall = 0x4;
        private const byte Field = 0x6;
        private const byte LocalSig = 0x7;
        private const byte Property = 0x8;
        private const byte ByRef = 0x10;
        private const byte TypedByRef = 0x16;

        /// <summary>
        /// 0x00: 
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public static Signature ParseMethodRef(byte[] blob)
        {
            return null;
        } 
    }
}