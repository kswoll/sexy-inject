# Property Injection

While constructor injection is preferable in general (since it makes the dependencies syntactally explicit), you can also opt-in to property injection.  There are various ways to go about it, but to start with, we'll inject some data into the settable properties of this class:

```
public class PropertyClass 
{
    public Dependency Dependency { get; set; }
    public string String { get; set; }
    public int Integer { get; set; }
}
```

And `Dependency` is just:

```
public class Dependency {}
```

Let's say we want to register a binding such that when an instance of this class is requested, it comes with the `String` property set to “foo” and the `Dependency` property set to an instance resolved through the registry:

```
registry.Bind<Dependency>();
registry.Bind<PropertyClass>().To()
    .InjectProperty(x => x.Dependency)
    .InjectProperty(x => x.String, _ => "foo");
```

We use two overloads of `InjectProperty`.  The first one, with only one argument, defaults to resolving the dependency through the registry as though you called `registry.Get<Dependency>()`.  The second one passes a lambda, allowing you to customize how that particular property is resolved.  In this scenario we just returned a constant, but you could obviously apply whatever logic you felt like to come up with a value.

You might have noticed we introduce that little naked call to `.To()`.  This is necessary because all the facilities for manipulating the binding is actually on the type returned by the `.To()` method (and its overloads).  That type is a class called `ResolverContext` and is also the type returned by all of its own methods to allow for method-chaining.

When using property injection, you might have a policy where you want all the public properties that have a setter to be injected with an appropriate dependency retrieved through the registry.  To do that, you could use the `.InjectProperties()` operator:

```
registry.Bind<string>().To(type => "foo");
registry.Bind<Dependency>();
registry.Bind<PropertyClass>().To().InjectProperties();
```

Finally, if you have your own heuristic you'd like to use to select properties, you can provide a filter to `InjectProperties` to decide for yourself into which properties you'd like to inject dependencies.  For example, perhaps you have a custom `InjectAttribute` you use to decorate properties into which you'd like dependencies injected.  In that scenario, your binding would look like:

```
registry.Bind<string>().To(type => "foo");
registry.Bind<Dependency>();
registry.Bind<PropertyClass>().To()
    .InjectProperties(x => x.IsDefined(typeof(InjectAttribute));
```
