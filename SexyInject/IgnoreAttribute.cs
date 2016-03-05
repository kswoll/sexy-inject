using System;

namespace SexyInject
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}