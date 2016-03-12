namespace SexyInject.Tests.TestClasses
{
    public class SecondLevelInjectionClass
    {
        public InjectionClass InjectionClass { get; }

        public SecondLevelInjectionClass(InjectionClass injectionClass)
        {
            InjectionClass = injectionClass;
        }
    }
}