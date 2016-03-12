using System;

namespace SexyInject.Tests.TestClasses
{
    public class SimpleClass : ISimpleClass
    {
        public string StringProperty { get; set; }
        public ISomeInterface SomeInterface { get; set; }
        public int IntProperty { get; set; }
        public bool BoolField;
        public readonly DateTime ReadonlyField = new DateTime(2016, 1, 1); 
    }
}