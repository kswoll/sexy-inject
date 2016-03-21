# Custom Behavior When Resolving an Instance

Sometimes you might want to jump into the resolve process and just applly arbitrary behavior on the instance.  You do that using the `WhenResolved` operator.  To demonstrate how it works, we'll just show another way of doing property injection. Earlier, we had a line that read:

```
    .InjectProperty(x => x.String, _ => "foo");
```

We could instead have used `WhenResolved` to set that property directly:

```
registry.Bind<PropertyClass>().To().WhenResolved(x => x.String = "foo");
```

The point here being that `WhenResolved` is more flexible — it allows you to do whatever you want with an instance after it has been resolved.  

