using System.Reflection;

namespace SexyInject
{
    /// <summary>
    /// A delegate used to provide a means to select a particular constructor on a class when using 
    /// the standard constructor injection pattern.  By default, of the constructors on a class with the most 
    /// parameters, one of them will be selected.  Typically, this is actually a pretty good formula, but if 
    /// your needs dictate more fine-grained control, you are generally offered an overload that allows
    /// you to supply an implementation of this delegate that explicitly specifies the particular 
    /// constructor you want to use.
    /// </summary>
    /// <param name="constructors">All the constructors on a particular type from which to choose</param>
    /// <returns>The actual constructor to use to instantiate the type.</returns>
    public delegate ConstructorInfo ConstructorSelector(ConstructorInfo[] constructors);
}