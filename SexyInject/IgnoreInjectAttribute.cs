using System;

namespace SexyInject
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class IgnoreInjectAttribute : Attribute
    {
    }
}