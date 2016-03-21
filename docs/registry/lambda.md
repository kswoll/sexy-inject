# Resolving Using Your Own Method or Lambda

The default constructor binding is often sufficient, but you can also provide your own logic to resolve a type.  As a trivial example, consider this binding that simply instantiates `Foo`:

```
registry.Bind<Foo>().To(targetType => new SimpleClass());
```

This overload of `To` allows you to supply a delegate taking one parameter (the “type being requested” — in this case `Foo` but that's not always the case) and another overload that allows you to supply two parameters, the `ResolveContext`, and the `targetType`.  More on that later.