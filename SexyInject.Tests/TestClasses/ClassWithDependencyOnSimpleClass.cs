namespace SexyInject.Tests.TestClasses
{
    public class ClassWithDependencyOnSimpleClass
    {
        public SimpleClass SimpleClass { get; }

        public ClassWithDependencyOnSimpleClass(SimpleClass simpleClass)
        {
            SimpleClass = simpleClass;
        }
    }
}