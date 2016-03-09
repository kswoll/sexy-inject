using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    public class ResolveContext
    {
        public Registry Registry { get; }
        public Type CallerType => GetCallerType();

        private readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();
        private readonly Func<Type, object[], object> resolver;
        private readonly Func<ResolveContext, Type, Func<ConstructorInfo[], ConstructorInfo>, object[], object> constructor;
        private readonly List<ResolveContextFrame> requestedTypeStack = new List<ResolveContextFrame>();

        private static readonly MethodInfo resolveMethod = typeof(ResolveContext).GetMethods().Single(x => x.Name == nameof(Resolve) && x.GetParameters().Length == 2);

        public ResolveContext(Registry registry, Func<Type, object[], object> resolver, Func<ResolveContext, Type, Func<ConstructorInfo[], ConstructorInfo>, object[], object> constructor, IEnumerable<object> arguments)
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
            if (!cache.TryGetValue(type, out result) && (!requestedTypeStack.LastOrDefault()?.TryGetArgument(type, out result) ?? true))
            {
                result = ProcessFrame(type, arguments, () => resolver(type, arguments));
                if (Registry.Bind(type).CachePolicy == CachePolicy.Transient) 
                    cache[type] = result;
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
        /// <returns>An expression that represents invoking the Resolve method.</returns>
        public static MethodCallExpression ResolveExpression(Expression resolveContext, Type type)
        {
            return Expression.Call(resolveContext, resolveMethod, Expression.Constant(type), Expression.Constant(new object[0]));
        }

        /// <summary>
        /// Create a new instance of the specified type using the registered rules for resolving dependencies.
        /// </summary>
        /// <param name="type">The type that should be instantiated.</param>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which 
        /// results in the selection of the first constructor with the most number of parameters.</param>
        /// <returns>A new instance of the specified type.</returns>
        public object Construct(Type type, Func<ConstructorInfo[], ConstructorInfo> constructorSelector)
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
        public object Construct(Type type, Func<ConstructorInfo[], ConstructorInfo> constructorSelector, params object[] arguments)
        {
            return ProcessFrame(type, arguments, () => constructor(this, type, constructorSelector, arguments));
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
        public T Construct<T>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector)
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
        public T Construct<T>(Func<ConstructorInfo[], ConstructorInfo> constructorSelector, params object[] arguments)
        {
            return (T)Construct(typeof(T), constructorSelector, arguments);
        }

        /// <summary>
        /// If a request is made to a type Foo that in turn has a constructor dependency on a type Bar then at the
        /// time that Bar is being resolved, GetParentRequestedType(0) will return typeof(Foo).
        /// </summary>
        /// <param name="index">How far up the stack of parent types to go.</param>
        /// <returns>One of the parent types that is in the process of being resolved.</returns>
        public Type GetCallerType(int index = 0)
        {
            var trueIndex = requestedTypeStack.Count - 2 - index;
            if (trueIndex < 0)
                return null;
            return requestedTypeStack[trueIndex].RequestedType;
        }

        public void InjectArgument(object argument)
        {
            requestedTypeStack.Last().InjectArgument(argument);
        }

        private object ProcessFrame(Type type, object[] arguments, Func<object> action)
        {
            requestedTypeStack.Add(new ResolveContextFrame(type, arguments));
            var result = action();
            requestedTypeStack.RemoveAt(requestedTypeStack.Count - 1);
            return result;
        }
    }
}