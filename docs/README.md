# SexyInject

SexyInject is a dependency injection framework.  It is designed to be flexible such that you can create very general bindings that satisfy a rich set of types.  For example, many DI frameworks have built-in support for injecting `Func<T>` into your types such that it ends up being a factory function for `T`.  While SexyInject also supports this natively, it is implemented as a simple extension method that you could have written yourself, which demonstrates that if the facility weren't already included, it would have been trivial for you to add that functionality yourself.  Later on, we'll take a look at that extension method and break it down piece by piece.

## Installation

Use NuGet, of course!

    Instal-Package SexyInject
    
## Quick Start

Generally speaking, the two main use-cases for a DI framework are to provide instances of classes on request, and to provide implementations of interfaces in like fashion.  SexyInject works both as a service locator (I want an instance of something, give it to me!) and as a dependency injector (My class has lots of dependencies: fulfill them for me!).  

Those two scenarios are not strictly mutually exclusive, since even with dependency injection, you need a bootstrap scenario that will give you an instance of a class with its dependencies resolved.  For example, in ASP.NET MVC, you're going to need an instance of a controller instantiated with its dependencies resolved.  That can't happen in pure DI -- you're going to need to "ask" for that instance from the "service locator" when trying to instantiate the controller.  From that point on, however, DI should take over and generally speaking you shouldn't have to talk to the service locator ever again.

So with that boilerplate out of the way, how do you use this framework?  The central class is named `Registry`.  It's the "container", so to speak. It is on this class that you register **bindings** which dictate precisely how a request for some type is resolved. It is also the class with which you interact in order to obtain an instance of such a type. (service-locator style; that will actually be the main style on which we focus, since DI is implicit in those scenarios)

So. Let's start from scratch. Let's create a registry and try to obtain an instance of a type from it:

    var registry = new Registry();
    var instance = registry.Get<Foo>();
    
That's the basic scenario. There's just one fly in the ointment. If you actually try this code it won't work. You'll get an exception complaining that the type `Foo` has not been registered. So let's make clear one important point from the start: with SexyInject *no type* will be available by default. You can't even request an instance of some ordinary type with a default constructor without getting rejected. But that's an important part of the design -- we want all the behavior provided by the framework to be purely opt-in. Fortunately, opting-in to such scenarios is easy!

### Typical Setup

So now that we've gotten out of the way the notion that you're going to have to configure a few things before you can even properly use the framework, let's demonstrate what a normal minimalist scenario might look like.

    var registry = new Registry();
    registry.RegisterImplicitPattern();
    
That second line is what allows the framework to resolve types that are obviously straightforward to realize. Things like classes with defualt constructors, or classes that have constructors with parameters that are similarly straightforward to resolve.  So with that, now we can have the line:

    var instance = registry.Get<Foo>();
    
Assuming `Foo` has a constructor that is either parameterless, or only has dependencies that are equally easy to resolve, that's all you have to do in terms of setup.

That being said, that is ignoring probably the most ubiquitous use-case for DI, resolving an interface into a particular implementation of that interface.  So let's tackle that now:

    registry.Bind<IFoo>(x => x.To<Foo>());
    
Here we say that for any request for an instance of the interface `IFoo`, provide an instance of the class `Foo` (which *must* implement that interface -- the generic type constraints will enforce this).

## Resolving

Service locator style:

```
registry.Get<IFoo>();
```

Dependency injection style:

```
public class Bar
{
    public Bar(IFoo foo)
    {
        ...
    }
}

registry.Get<Bar>();    // The parameter "foo" will be resolved using dependency injection
```

Lazy:

```
var lazyFoo = registry.Get<Lazy<IFoo>>();
var foo = lazyFoo.Value;    // Will only instantiate or otherwise resolve "foo" at this point
```

Factory:

```
var fooFactory = registry.Get<Func<IFoo>>();
```

## Partial Application

One of the more unique features of SexyInject is that you can request a factory that allows injection of dependencies for arguments that were ommitted via default parameters. In other words, if you invoke a constructor and omit certain parameters and allow the default values be used (whether via named arguments or, truncating the argument list) then those default values will instead be injected via the registry.  

The use-case is a conventional front-end in which you constantly new up forms and generally need to pass in state to that new form (for example, the selected item for which you want to show a detail view).  Conventional DI works great for scenarios where the central class could not receive any paramterized state, like ASP.NET, both MVC and WebAPI.  Technically state is passed in, but it's on top of an infrastructure in which that state is available via properties on the controller.  This works -- but it's often not how you really want to code these interactions.  Often, you just want to create a new window and simultaneously inject the dependencies that are available via the DI framework, while still passing in some local state, such as the selected item.

To enable this feature:

    registry.RegisterPartialApplication();
    
Then, supposing you have a class such as:

    public class ItemWindow
    {
        public Item Item { get; }
        public IEmailService EmailService { get; }
        
        public ItemWindow(Item item, IEmailService emailService = null)
        {
            Item = item;
            EmailService = emailService;
        }
    }
    
Then you can instantiate this via:

    registry.Construct(_ => new ItemWindow(item));
    
In a more DI friendly scenario, just ask for either a `Func<Func<T>>`, where `T` is the type you are trying to create, or use the included delegate `PartialConstructor<T>`, which scans a bit better.

## Transient Caching

By default, each time you resolve an instance, dependency injection will only look up a dependency on a given type one time.  For example:

```
public class Bar
{
    public Bar(Foo foo, FooBar fooBar) {}
}

public class Foo 
{
    public Foo(FooBar fooBar) {}
}

registry.Resolve<Bar>();
```

In this scenario, both `Bar` and `Foo` have a dependency on `FooBar`, and both constructors will be passed the same instance of `FooBar` vs. creating a separate instance for each constructor.  Importantly, a subsequent call to `registry.Resolve<Bar>()` will start the process over and not re-use any existing instances (unless you are caching things during registration as in the case of singletons).

If you would like to avoid this behavior, then you must explicitly opt-out during registration:

```
registry.Bind<FooBar>(CachePolicy.Never);
```

The default behavior is equivalent to:

```
registry.Bind<FooBar>(CachePolicy.Transient);
```












