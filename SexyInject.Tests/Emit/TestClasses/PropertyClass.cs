using System;

namespace SexyInject.Tests.Emit.TestClasses
{
    public class PropertyClass
    {
        public byte ByteProperty { get; set; }
        public short ShortProperty { get; set; }
        public int IntProperty { get; set; }
        public long LongProperty { get; set; }
        public sbyte SByteProperty { get; set; }
        public ushort UShortProperty { get; set; }
        public uint UIntProperty { get; set; }
        public ulong ULongProperty { get; set; }
        public bool BoolProperty { get; set; }
        public string StringProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }

        public static int StaticIntProperty { get; set; }

        public int this[int index]
        {
            get { return 0; }
            set {  }
        }
    }
}