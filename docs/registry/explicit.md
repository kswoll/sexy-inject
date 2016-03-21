# Explicit Injection

You can also inject dependencies explicitly using the `InjectArgument` operator.  This allows you to provide an instance of an object and make it available as a dependency to inject into the bound type.

```
registry
    .Bind<Consumer1>().To()
    .Inject(_ => new StringClass("value1"));
registry
    .Bind<Consumer2>().To()
    .Inject(_ => new StringClass("value2"));
```

As you can see, we're saying for `Consumer1`, provide an instance of `StringClass` to its constructor with its property set to “value1”.  In contrast, when obtaining an instance of `Consumer2` it will have been provided an instance with its property set to “value2”.
