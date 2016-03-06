namespace SexyInject.Tests.TestClasses
{
    public class ClassWithDependencyOnOtherClassWithDependencyOnSimpleClass
    {
        public SimpleClass SimpleClass { get; }
        public ClassWithDependencyOnSimpleClass ClassWithDependencyOnSimpleClass { get; }

        public ClassWithDependencyOnOtherClassWithDependencyOnSimpleClass(SimpleClass simpleClass, ClassWithDependencyOnSimpleClass classWithDependencyOnSimpleClass)
        {
            SimpleClass = simpleClass;
            ClassWithDependencyOnSimpleClass = classWithDependencyOnSimpleClass;
        }
    }
}