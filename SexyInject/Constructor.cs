using System;

namespace SexyInject
{
    public delegate object Constructor(Type type, ConstructorSelector constructorSelector = null);
}