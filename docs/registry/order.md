# Composition and Order of Operations

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

