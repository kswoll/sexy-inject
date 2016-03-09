using System;
using System.Collections.Generic;

namespace SexyInject
{
    public class ResolveContextFrame
    {
        public Type RequestedType { get; }

        private Dictionary<Type, object> arguments;

        public ResolveContextFrame(Type requestedType, object[] arguments)
        {
            RequestedType = requestedType;
            foreach (var argument in arguments) 
                InjectArgument(argument);
        }

        public void InjectArgument(object argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));
            if (arguments?.ContainsKey(argument.GetType()) ?? false)
                throw new ArgumentException("Cannot pass more than one argument of the same type", nameof(argument));
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