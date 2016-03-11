namespace SexyInject.Tests.TestClasses
{
    public class SimpleClass : ISimpleClass
    {
        public string StringProperty { get; set; }
        public ISomeInterface SomeInterface { get; set; }
        public int IntProperty { get; set; }
    }
}