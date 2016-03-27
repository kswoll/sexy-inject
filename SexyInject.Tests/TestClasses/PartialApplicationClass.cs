namespace SexyInject.Tests.TestClasses
{
    public class PartialApplicationClass
    {
        public int Int1 { get; }
        public int Int2 { get; }
        public string String1 { get; }
        public string String2 { get; }
        public SimpleClass SimpleClass { get; }

        public PartialApplicationClass(int int1, int int2, string string1, string string2, SimpleClass simpleClass = null, SomeStruct someStruct = default(SomeStruct))
        {
            Int1 = int1;
            Int2 = int2;
            String1 = string1;
            String2 = string2;
            SimpleClass = simpleClass;
        }
    }
}