using System;
using System.Collections.Generic;

namespace SexyInject
{
    public class ResolveContextFrame
    {
        public Type RequestedType { get; }

        private Dictionary<Type, object> arguments;

        public ResolveContextFrame(Type requestedType)
        {
            RequestedType = requestedType;
        }

        public void InjectArgument(object argument)
        {
            arguments = arguments ?? new Dictionary<Type, object>();
        }

        public bool TryGetArgument(Type argumentType, out object result)
        {
            if (arguments == null)
            {
                result = null;
                return false;
            }
            else if (arguments.TryGetValue(argumentType, out result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}