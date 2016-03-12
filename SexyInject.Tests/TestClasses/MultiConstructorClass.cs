namespace SexyInject.Tests.TestClasses
{
    public class MultiConstructorClass
    {
        public SimpleClass SimpleClass { get; }
        public ISomeInterface SomeInterface { get; }

        public MultiConstructorClass(SimpleClass simpleClass)
        {
            SimpleClass = simpleClass;
        }

        public MultiConstructorClass(ISomeInterface someInterface)
        {
            SomeInterface = someInterface;
        }
    }
}