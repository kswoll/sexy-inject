using System;

namespace SexyInject
{
    /// <summary>
    /// Formalizes the lambda used for instantiating types.
    /// </summary>
    /// <param name="type">The type to instantiate.</param>
    /// <param name="constructorSelector">A lambda that selects one of the constructors defined on type.  If null, 
    /// one of the constructors with highest number of parameters will be used</param>
    /// <returns>An instance of the specified type</returns>
    public delegate object Constructor(Type type, ConstructorSelector constructorSelector = null);

    public delegate T Constructor<T>(Func<ResolveContext, T> factory);
}