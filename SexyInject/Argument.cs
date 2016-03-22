namespace SexyInject
{
    public struct Argument
    {
        public object Value { get; }
        public ArgumentType ArgumentType { get; }

        public Argument(object value, ArgumentType argumentType)
        {
            Value = value;
            ArgumentType = argumentType;
        }
    }
}