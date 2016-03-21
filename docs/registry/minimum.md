# Minimum Registration
Starting with a vanilla instance, what is the simplest possible example of binding something yourself?  Consider a class `Foo`:

```
public class Foo 
{
}
```

And requesting an instance of it:

```
var foo = registry.Get<Foo>();
```

Without any bindings set up, you'd simply get an exception indicating that no binding has been registered for type `Foo`. To create a binding so that a new instance is returned upon request, you'd add the line:

```
registry.Bind<Foo>();
```

Without doing anything else, this will just use the standard constructor and instantiate it.  This is the simplest possible binding, but not terribly interesting.  Furthermore, if you've called `RegisterImplicitPattern` then this happens for you automatically. 