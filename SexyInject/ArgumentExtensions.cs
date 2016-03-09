using System.Linq;

namespace SexyInject
{
    public static class ArgumentExtensions
    {
        public static Argument[] ToArguments(this object[] array, ArgumentType argumentType)
        {
            return array.Select(x => new Argument(x, argumentType)).ToArray();
        }
    }
}