# Registry

The main class you interact with is an instance of `Registry`.   It is here where you specify how requests for some type `T` should be handled.  You might, for example, want to register an interface to resolve to some particular implementation. Or, you might want all requests for a type to resolve into the same instance (a singleton pattern).  Or all requests for an instantiatable type (i.e. non-abstract, etc.) to result in an instance of that type.  

That last example might seem curious — most DI frameworks support that without your having to do any work.  However, with this framework, a vanilla instance of `Registry` will never resolve any requests for any type.  It will simply throw an exception.  Since enabling various features of this sort are trivial one-liners, SexyInject adheres to an opt-in model where all the facilities you desire are explicitly enabled by you.  As an example, to enable the behavior where instantiatable types are automatically created when requested, you simply need this code:

```
var registry = new Registry();
registry.RegisterImplicitPattern();
```

Similarly, registering `Func<T>` factories only requires:

```
var registry = new Registry();
registry.RegisterFactoryPattern();
```














## Custom Behavior When Resolving an Instance

Sometimes you might want to jump into the resolve process and just applly arbitrary behavior on the instance.  You do that using the `WhenResolved` operator.  To demonstrate how it works, we'll just show another way of doing property injection. Earlier, we had a line that read:

```
    .InjectProperty(x => x.String, _ => "foo");
```

We could instead have used `WhenResolved` to set that property directly:

```
registry.Bind<PropertyClass>().To().WhenResolved(x => x.String = "foo");
```

The point here being that `WhenResolved` is more flexible — it allows you to do whatever you want with an instance after it has been resolved.  

## Composition and Order of Operations

As you've seen, you can string together many operators in a row to fashion a binding.   The order in which you compose them often doesn't matter, but it can sometimes have important consequences.  The operators are applied in reverse order, meaning that the last operator you use will be executed first.  To illustrate this, let's look at a contrived example that composes the operators `Cache` and `WhenResolved`.  

```
registry.Bind<PropertyClass>().To().Cache(Cache.Singleton).WhenResolved(x => x.Integer++);
registry.Get<PropertyClass>();
var instance = registry.Get<PropertyClass>();
```

What will the value of `instance.Integer` be? We call `registry.Get<PropertyClass>()` twice, so given we said evaluation happens in reverse order, that means `WhenResolved` will be called twice on the cached instance of `PropertyClass`.  Therefore, the value of the property will be 2.  Conversely, if we set up the binding in the opposite order:

```
registry.Bind<PropertyClass>().To().WhenResolved(x => x.Integer++).Cache(Cache.Singleton);
registry.Get<PropertyClass>();
var instance = registry.Get<PropertyClass>();
```

On the first invocation `registry.Get<PropertyClass>()`, the item is not found in the cache and so it proceeds up the chain and the `WhenResolved` operator increments the property. On the second invocation to the item is found in the cache and immediately returned, thus shortcircuiting the binding and omitting the call to the `WhenResolved` operator.  Therefore, the value of the property in this case is 1.

You can use this to your advantage as it allows you, for example, to determine whether you want behavior to happen each time an instance is resolved — regardless of whether it has been cached already — or only the first time it is instantiated.

## Resolving

As you've seen already, there's not much to it when it comes to requesting instances of a type.  You can use the framework as a service locator, as the examples above have shown.  Obviously you can also use it as a conventional DI framework and have your dependencies injected into your class.  So say you have a new class, `Bar`, that looks like this:

```
public class Bar
{
    public Foo Foo { get; }

    public Bar(Foo foo)
    {
        Foo = foo;
    }
}
```

Assuming you've registered implicit binding, when you request an instance of `Bar`, this class would be instantiated and passed an instance of `Foo`.  The instance of `Foo` will be obtained from the registry and resolved as normal.  That may mean it's a fresh instance or that type or an instance obtained through some other heuristic you've supplied via registration such as a singleton.

One subtle detail is that when getting an instance from the registry, only one instance of a particular type will be resolved. To make this more clear, consider another type:

```
public class FooBar
{
    public Foo Foo { get; }
    public Bar Bar { get; }

    public FooBar(Foo foo, Bar bar)
    {
        Foo = foo;
        Bar = bar;
    }
}
```

Now to provide you with an instance of `FooBar`, an instance of `Foo` must be provided to `FooBar` and furthermore, an instance of `Foo` must be provided to `Bar`.  By default, only one instance of `Foo` will be resolved and that instance will be provided to the constructors of both `Bar` and `FooBar`.  You can opt out of this behavior when registering by supplying a cache policy:

```
registry.Bind<Foo>(CachePolicy.Never);
```

```
registry.Bind<IFoo>(x => x.To<Foo>().When(_ => true).Cache());
registry.Bind<IFoo>().To(x => x.When(_ => true).Cache());
```

Now instances will never be (transiently) cached and you will get a separate instance for each class.  Keep in mind that this caching only applies to a single invocation to `Register.Get`.  Subsequent invocations will get their own cache and the cache will not live longer that that.


```
public static void RegisterFactoryPattern(this Registry registry)
{
    registry
        .Bind(typeof(Func<>))
        .To((context, targetType) => Expression.Lambda(targetType, registry.GetExpression(targetType.GetGenericArguments()[0])).Compile())
        .Cache((context, targetType) => targetType);
}
```

