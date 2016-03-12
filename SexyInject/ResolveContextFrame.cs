using System;
using System.Collections.Generic;
using System.Linq;

namespace SexyInject
{
    public class ResolveContextFrame
    {
        public Type RequestedType { get; }

        private readonly ResolveContext context;
        private Dictionary<Type, object> arguments;

        public ResolveContextFrame(ResolveContext context, Type requestedType, object[] arguments)
        {
            this.context = context;

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

            foreach (var current in context.EnumerateTypeHierarchy(argument.GetType()).Where(x => x != typeof(object) && x != typeof(ValueType) && x != typeof(Enum)))
            {
                if (!arguments.ContainsKey(current))
                    arguments[current] = argument;
                arguments[argument.GetType()] = argument;
            }
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