## Generalizing Registration

Sometimes you'll want to provide binding logic that is general enough that it is suitable for a variety of types.  In fact, one of the most general rules of all is the one mentioned earlier, `RegisterImplicitPattern`.  Its implementation is straightforward:

```
Func<Type, bool> isInstantiatable = type => !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
registry
    .Bind<object>()
    .To((context, type) => context.Constructor(type))
    .When((context, targetType) => isInstantiatable(targetType));
```

To break this down:

* We are registering a binding for `object` which will catch all types as a fallback assuming a binding for a more specific type is not found.
* We are using the lambda pattern to provide an instance by using the `Constructor` function on the context.  This is subtly different from a separate method, `Construct`, which will be explained in detail later.
* We are constraining the binding to only those types which we could possibly instantiate — i.e. not abstract, an interface, etc.

The basic idea here is that if a type is requested for which no binding has been provided for that type itself, the type's hierarchy will be traversed.  To put it another way, each type in the following sequence will be traversed in order until a binding has been found for that type:

1. The requested type itself
2. If the requested type is a generic type, then its generic type definition
3. Each implemented interface of that type
4. The base type of that type, followed by its base type, all the way to `object`

In sum, you can see how given the right circumstances, you can create a binding that will have broad applicability.  This leads us to the next pattern for `Func<T>` which allows you to inject a factory for a type so that you may request an instance on demand.