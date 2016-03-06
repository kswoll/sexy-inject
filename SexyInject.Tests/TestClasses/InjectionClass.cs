namespace SexyInject.Tests.TestClasses
{
    public class InjectionClass
    {
        public SimpleClass SimpleClass { get; }

        public InjectionClass(SimpleClass simpleClass)
        {
            SimpleClass = simpleClass;
        }
    }
}