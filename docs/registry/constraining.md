# Constraining Bindings

Consider the following types:

```
public class StringClass
{
    public string StringProperty { get; }

    public StringClass(string stringProperty)
    {
        StringProperty = stringProperty;
    }
}

public class Consumer1
{
    public StringClass StringClass { get; }

    public Consumer1(StringClass stringClass)
    {
        StringClass = stringClass;
    }
}

public class Consumer2
{
    public StringClass StringClass { get; }

    public Consumer2(StringClass stringClass)
    {
        StringClass = stringClass;
    }
}
```

Now suppose we wanted a rule that makes it so when a request for `Consumer1` is made, an instance of `StringClass` is provided with the `StringProperty` set to “value1” and when a request for `Consumer2` is made, an instance of `StringClass` is provided with the `StringProperty` set to “value2”.  The binding for that could look like:

```
registry
    .Bind<StringClass>()
    .To(_ => new StringClass("value1"))
    .When((context, targetType) => context.CallerType == typeof(Consumer1));
registry
    .Bind<StringClass>()
    .To(_ => new StringClass("value2"))
    .When((context, targetType) => context.CallerType == typeof(Consumer2));
```

This is obviously a contrived example, and was mostly intended to demonstrate the `When` operator used for constraining a binding based on an arbitrary predicate you provide.  There are other ways to accomplish this sort of task, and the pattern you use will depend on your circumstances and taste.  Let's consider next an alternative solution.