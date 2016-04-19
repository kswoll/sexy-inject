using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SexyInject.Emit;

namespace SexyInject
{
    public class ResolveContext
    {
        public Registry Registry { get; }
        public Type CallerType => GetCallerType();
        public Constructor Constructor => constructor;

        private readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();
        private readonly Func<Type, object> resolver;
        private readonly Constructor constructor;
        private readonly List<ResolveContextFrame> frames = new List<ResolveContextFrame>();

        private static readonly MethodInfo resolveMethod = typeof(ResolveContext).GetMethods().Single(x => x.Name == nameof(Resolve) && x.GetParameters().Length == 2);

        internal static MethodInfo ResolveMethod => resolveMethod;

        public ResolveContext(Registry registry, Func<Type, object> resolver, Constructor constructor, IEnumerable<object> arguments)
        {
            Registry = registry;
            this.resolver = resolver;
            this.constructor = constructor;

            foreach (var argument in arguments)
            {
                if (argument == null)
                    throw new ArgumentException("Arguments array cannot contain null elements.", nameof(arguments));
                var argumentType = argument.GetType();
                if (cache.ContainsKey(argumentType))
                    throw new ArgumentException("Only one argument of the same type may be provided.", nameof(arguments));
                cache[argumentType] = argument;
            }
        }

        public void Cache(Type type, object o)
        {
            cache[type] = o;
        }

        public bool TryRetrieveFromCache(Type type, out object result)
        {
            return cache.TryGetValue(type, out result);
        }

        /// <summary>
        /// Resolves the specified type using the binding rules specified in the registry.  When constructing binding rules
        /// that provide a context, you should always use this method to resolve types rather than through Registry.Get.  This
        /// is because when resolving a single request it is important to maintain the same ResolveContext and a separate call
        /// to Registry.Get will lead to a new instance, which will cause unexpected behavior.
        /// </summary>
        /// <typeparam name="T">The type that should be resolved.</typeparam>
        /// <returns>An instance of the specified type</returns>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Resolves the specified type using the binding rules specified in the registry.  When constructing binding rules
        /// that provide a context, you should always use this method to resolve types rather than through Registry.Get.  This
        /// is because when resolving a single request it is important to maintain the same ResolveContext and a separate call
        /// to Registry.Get will lead to a new instance, which will cause unexpected behavior.
        /// </summary>
        /// <typeparam name="T">The type that should be resolved.</typeparam>
        /// <returns>An instance of the specified type</returns>
        public T Resolve<T>(params object[] arguments)
        {
            return (T)Resolve(typeof(T), arguments);
        }

        /// <summary>
        /// Resolves the specified type using the binding rules specified in the registry.  When constructing binding rules
        /// that provide a context, you should always use this method to resolve types rather than through Registry.Get.  This
        /// is because when resolving a single request it is important to maintain the same ResolveContext and a separate call
        /// to Registry.Get will lead to a new instance, which will cause unexpected behavior.
        /// </summary>
        /// <param name="type">The type to resolve through the registry</param>
        /// <param name="arguments">An array of objects that will be used when injecting dependencies into the target type.  
        /// The scope of these arguments is localized to resolving this one instance vs. any other dependencies that might
        /// request an instance of the same type.</param>
        /// <returns>An instance of the specified type</returns>
        public object Resolve(Type type, params object[] arguments)
        {
            object result;
            if (!cache.TryGetValue(type, out result) && (!frames.LastOrDefault()?.TryGetArgument(type, out result) ?? true))
            {
                result = ProcessFrame(type, arguments, () => resolver(type));
            }
            return result;
        }

        /// <summary>
        /// Useful for creating expression trees that invoke the Resolve method.  This can often be useful for creating 
        /// dynamically generated lambdas.
        /// </summary>
        /// <param name="resolveContext">The expression that represents the instance of ResolveContexet upon which you want
        /// to invoke the Resolve method.</param>
        /// <param name="type">The type to pass to the Resolve method.</param>
        /// <param name="arguments">The arguments array that will be passed to the resolve method.</param>
        /// <returns>An expression that represents invoking the Resolve method.</returns>
        public static MethodCallExpression ResolveExpression(Expression resolveContext, Type type, Expression arguments)
        {
            return Expression.Call(resolveContext, resolveMethod, Expression.Constant(type), arguments);
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <param name="type">The type that should be instantiated.</param>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object Construct(Type type, ConstructorSelector constructorSelector)
        {
            return Construct(type, constructorSelector, new object[0]);
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <param name="type">The type that should be instantiated.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object Construct(Type type)
        {
            return Construct(type, null, new object[0]);
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <param name="type">The type that should be instantiated.</param>
        /// <param name="arguments">An array of objects that will be used when injecting dependencies into the target type.  
        /// The scope of these arguments is localized to resolving this one instance vs. any other dependencies that might
        /// request an instance of the same type.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object Construct(Type type, params object[] arguments)
        {
            return Construct(type, null, arguments);
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <param name="type">The type that should be instantiated.</param>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        /// <param name="arguments">An array of objects that will be used when injecting dependencies into the target type.  
        /// The scope of these arguments is localized to resolving this one instance vs. any other dependencies that might
        /// request an instance of the same type.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object Construct(Type type, ConstructorSelector constructorSelector, params object[] arguments)
        {
            return ProcessFrame(type, arguments, () => constructor(type, constructorSelector));
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <typeparam name="T">The type that should be instantiated.</typeparam>
        /// <returns>A new instance of T.</returns>
        public T Construct<T>()
        {
            return (T)Construct(typeof(T));
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <typeparam name="T">The type that should be instantiated.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        /// <returns>A new instance of T.</returns>
        public T Construct<T>(ConstructorSelector constructorSelector)
        {
            return (T)Construct(typeof(T), constructorSelector);
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <typeparam name="T">The type that should be instantiated.</typeparam>
        /// <param name="arguments">An array of objects that will be used when injecting dependencies into the target type.  
        /// The scope of these arguments is localized to resolving this one instance vs. any other dependencies that might
        /// request an instance of the same type.</param>
        /// <returns>A new instance of T.</returns>
        public T Construct<T>(params object[] arguments)
        {
            return (T)Construct(typeof(T), arguments);
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <typeparam name="T">The type that should be instantiated.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        /// <param name="arguments">An array of objects that will be used when injecting dependencies into the target type.  
        /// The scope of these arguments is localized to resolving this one instance vs. any other dependencies that might
        /// request an instance of the same type.</param>
        /// <returns>A new instance of T.</returns>
        public T Construct<T>(ConstructorSelector constructorSelector, params object[] arguments)
        {
            return (T)Construct(typeof(T), constructorSelector, arguments);
        }

        public T Construct<T>(Func<ResolveContext, T> constructor)
        {
            var factory = PartialApplicationFactory.CreateDelegate(constructor);
            return factory(this);
        }

        public object Construct(Type type, Delegate constructor)
        {
            var factory = PartialApplicationFactory.CreateDelegate(type, constructor);
            return factory.DynamicInvoke(this);
        }

        /// <summary>
        /// If a request is made to a type Foo that in turn has a constructor dependency on a type Bar then at the
        /// time that Bar is being resolved, GetParentRequestedType(0) will return typeof(Foo).
        /// </summary>
        /// <param name="index">How far up the stack of parent types to go.</param>
        /// <returns>One of the parent types that is in the process of being resolved.</returns>
        public Type GetCallerType(int index = 0)
        {
            var trueIndex = frames.Count - 2 - index;
            if (trueIndex < 0)
                return null;
            return frames[trueIndex].RequestedType;
        }

        private object ProcessFrame(Type type, object[] arguments, Func<object> action)
        {
            frames.Add(new ResolveContextFrame(this, type, arguments));
            var result = action();
            frames.RemoveAt(frames.Count - 1);
            return result;
        }

        public void InjectArgument(object argument)
        {
            frames.Last().InjectArgument(argument);
        }

        public IEnumerable<Type> EnumerateTypeHierarchy(Type type)
        {
            yield return type;
            if (type.IsGenericType)
                yield return type.GetGenericTypeDefinition();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                yield return type.GetGenericArguments()[0];

            foreach (var @interface in type.GetInterfaces())
            {
                yield return @interface;
                if (@interface.IsGenericType)
                    yield return @interface.GetGenericTypeDefinition();
            }

            var current = type.BaseType;
            while (current != null)
            {
                yield return current;

                if (current.IsGenericType)
                    yield return current.GetGenericTypeDefinition();

                current = current.BaseType;
            }
        }
    }
}