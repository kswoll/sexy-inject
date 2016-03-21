# Caching

When creating generalized bindings as described in the last section, you occasionally need to perform some expensive operations for each type you are capable of resolving.  In such a situation, it often becomes desirable to cache things — in this case caching once per type.  To illustrate this concept, we'll examine the implementation of `RegisterFactoryPattern`.

```
registry
    .Bind(typeof(Func<>))
    .To((context, targetType) => Expression.Lambda(
        targetType, 
        registry.GetExpression(targetType.GetGenericArguments()[0])).Compile()
    )
.Cache((context, targetType) => targetType);
```

Remember, the point of the factory pattern is to be able to inject an instance of `Func<T>` that when invoked will return an instance that has been resolved through your registry (generally speaking, this will return a new instance, but there's nothing stopping you from returning a singleton, or an instance derived from any other heuristic).

Now, before we completely break down this code, let's think about what that means in practice.  If you inject a `Func<StringClass>`, then there's no way to generically provide such a func without delving into some sort of dynamic code generation.  The simplest way to generate such code is by using expression trees in conjunction with `LambdaExpression`'s awesome `Compile` method.  Thus, by using expression trees we can rather trivially generate the `Func<T>`.  With that, let's now get into describing how `RegisterFactoryPattern` works.

1. We bind to `Func<>` so that when any request is made for *any* construction of it, this binding will be used.
2. We use the lambda overload of `.To` to provide a dynamically generated implementation of `Func<T>` that obtains an instance of `T` through the registry.  (`registry.GetExpression` is a helper method that returns an `Expression` representing an invocation to `Registry.Get`)
3. Finally, the `Cache` operator.  It's kind of like the LINQ operator `.GroupBy` in that you provide a lambda that returns a common “key” such that when the same key is returned the same instance will be returned.  So this is useful here because compiling an expression tree — while not *that* expensive — is still not something you want to do a million times for a million requests for `Func<T>` when a single instance will suffice.  This way, whenever an instance of `Func<T>` is requested, the same dynamically generated expression tree will be used instead of being regenerated each time.

