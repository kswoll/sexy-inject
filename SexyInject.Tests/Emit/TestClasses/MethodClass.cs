namespace SexyInject.Tests.Emit.TestClasses
{
    public class MethodClass
    {
        public void VoidMethod()
        {
        }

        public int IntMethod()
        {
            return 0;
        }

        public int IntMethodIntParameter(int param1)
        {
            return 0;
        }

        public T GenericMethod<T>(T param1)
        {
            return param1;
        }
    }
}