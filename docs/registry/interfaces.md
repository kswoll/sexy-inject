# Associating an Interface With an Implementation

Let's move on to the canonical example of associating an implementation of an interface with that interface.  Let's assume you have the interface `IFoo`:

```
public interface IFoo
{
}
```

To make it so that requests for instances of `IFoo` return an instance of the class `Foo`, you'd add the following binding:

```
registry.Bind<IFoo>().To<Foo>();
```

Now, when you invoke `registry.Get<IFoo>()` you'll be returned an instance of `Foo`.
