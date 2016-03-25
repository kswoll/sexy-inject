using System.Reflection;

namespace SexyInject.Emit
{
    public class MethodBaseILProvider : IILProvider
    {
        private readonly MethodBase method;
        private byte[] byteArray;

        public MethodBaseILProvider(MethodBase method)
        {
            this.method = method;
        }

        public byte[] GetByteArray()
        {
            if (byteArray == null)
            {
                var methodBody = method.GetMethodBody();
                byteArray = methodBody == null ? new byte[0] : methodBody.GetILAsByteArray();
            }
            return byteArray;
        }
    }
}