namespace SexyInject.Tests.TestClasses
{
    public class MultiConstructorDependent
    {
        public MultiConstructorClass MultiConstructorClass { get; }

        public MultiConstructorDependent(MultiConstructorClass multiConstructorClass)
        {
            MultiConstructorClass = multiConstructorClass;
        }
    }
}