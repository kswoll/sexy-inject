using System;

namespace SexyInject
{
    /// <summary>
    /// This is the first class you interact with in the body of your Bind handler.  It has a series of
    /// overloads of the method "To" that allows you to specify how the type will be resolved.  For example
    /// you can specify a type and it will find a constructor to use.  Alternatively you may specify a
    /// lambda that returns an instance yourself.  Once you've invoked the To method, it returns an instance
    /// of ResolverContext with which you can specify the details of the binding in depth.
    /// </summary>
    public class Binder
    {
        protected readonly Binding binding;
        private bool isUsed;

        public Binder(Binding binding)
        {
            this.binding = binding;
        }

        protected ResolverContext AddResolver(IResolver resolver)
        {
            var context = new ResolverContext(binding.Registry, binding, resolver);
            AddResolverContext(context);
            return context;
        }

        protected void AddResolverContext(ResolverContext context)
        {
            if (isUsed)
                throw new InvalidOperationException("Cannot call To more than once on the same binding.");
            isUsed = true;

            binding.AddResolverContext(context);
        }

        protected void InsertResolverContext(ResolverContext context)
        {
            if (isUsed)
                throw new InvalidOperationException("Cannot call To more than once on the same binding.");
            isUsed = true;

            binding.InsertResolverContext(context);
        }

        /// <summary>
        /// Same as <see cref="To{T}(ConstructorSelector)" /> -- but a more concise syntax when you want to resolve to the bound type
        /// and apply further operators without explicitly supplying the same type for which the binding is being registered.
        /// </summary>
        public ResolverContext To(ConstructorSelector constructorSelector = null)
        {
            return To(binding.Type, constructorSelector);
        }

        /// <summary>
        /// Binds requests for the bound type to an instance of specified type.
        /// </summary>
        /// <param name="type">The type to which requests for the bound type should resolve</param>
        public ResolverContext To(Type type)
        {
            return AddResolver(new ConstructorResolver(type));
        }

        /// <summary>
        /// Binds requests for the bound type to an instance of specified type.
        /// </summary>
        /// <param name="type">The type to which requests for the bound type should resolve</param>
        /// <param name="constructorSelector">A callback to select the constructor on the specified type to use when instantiating
        /// it.  Defaults to null which results in the selection of the first constructor with the most number of parameters.</param>
        public ResolverContext To(Type type, ConstructorSelector constructorSelector)
        {
            return AddResolver(new ConstructorResolver(type, constructorSelector));
        }

        /// <summary>
        /// Binds requests for T to an instance of TTarget.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) to instantiate when an instance of T is requested.</typeparam>
        public ResolverContext To<TTarget>()
        {
            return To(typeof(TTarget), null);
        }

        /// <summary>
        /// Binds requests for T to an instance of TTarget.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) to instantiate when an instance of T is requested.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which
        /// results in the selection of the first constructor with the most number of parameters.</param>
        public ResolverContext To<TTarget>(ConstructorSelector constructorSelector)
        {
            return To(typeof(TTarget), constructorSelector);
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public ResolverContext To<TTarget>(Func<ResolveContext, Type, TTarget> resolver)
        {
            return AddResolver(new LambdaResolver((x, targetType) => resolver(x, targetType)));
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public ResolverContext To<TTarget>(Func<Type, TTarget> resolver)
        {
            return AddResolver(new LambdaResolver((context, targetType) => resolver(targetType)));
        }

        /// <summary>
        /// Binds requests for T to a specific instance
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="instance">The singleton instance to always return when type T is requested.</param>
        public ResolverContext To<TTarget>(TTarget instance)
        {
            return AddResolver(new LambdaResolver((context, type) => instance));
        }

    }

    public class Binder<T> : Binder
    {
        public Binder(Binding binding) : base(binding)
        {
        }

        public new ResolverContext<T> AddResolver(IResolver resolver)
        {
            var context = new ResolverContext<T>(binding.Registry, binding, resolver);
            AddResolverContext(context);
            return context;
        }

        public ResolverContext<T> InsertResolver(IResolver resolver)
        {
            var context = new ResolverContext<T>(binding.Registry, binding, resolver);
            InsertResolverContext(context);
            return context;
        }

        /// <summary>
        /// Binds requests for T to an instance of TTarget.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) to instantiate when an instance of T is requested.</typeparam>
        /// <param name="constructorSelector">A callback to select the constructor on TTarget to use when instantiating TTarget.  Defaults to null which
        /// results in the selection of the first constructor with the most number of parameters.</param>
        public new ResolverContext<T> To<TTarget>(ConstructorSelector constructorSelector = null)
            where TTarget : T
        {
            return AddResolver(new ConstructorResolver(typeof(TTarget), constructorSelector));
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public new ResolverContext<T> To<TTarget>(Func<Type, TTarget> resolver)
            where TTarget : T
        {
            return AddResolver(new LambdaResolver((context, targetType) => resolver(targetType)));
        }

        /// <summary>
        /// Binds requests for T to the result of a lambda function.
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="resolver">The lambda function that returns the instance of the reuqested type.</param>
        public new ResolverContext<T> To<TTarget>(Func<ResolveContext, Type, TTarget> resolver)
            where TTarget : T
        {
            return AddResolver(new LambdaResolver((context, type) => resolver(context, type)));
        }

        /// <summary>
        /// Binds requests for T to a specific instance
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="instance">The singleton instance to always return when type T is requested.</param>
        public new ResolverContext<T> To<TTarget>(TTarget instance)
            where TTarget : T
        {
            return AddResolver(new LambdaResolver((context, type) => instance));
        }

        /// <summary>
        /// Binds requests for T to a specific instance
        /// </summary>
        /// <typeparam name="TTarget">The subclass of T (or T itself) that is returned when an instance of T is requested.</typeparam>
        /// <param name="instance">The singleton instance to always return when type T is requested.</param>
        public new ResolverContext<T> OverrideTo<TTarget>(TTarget instance)
            where TTarget : T
        {
            return InsertResolver(new LambdaResolver((context, type) => instance));
        }

        /// <summary>
        /// Same as <see cref="To{T}(ConstructorSelector)" /> -- but a more concise syntax when you want to resolve to the bound type
        /// and apply further operators without explicitly supplying the same type for which the binding is being registered.
        /// </summary>
        public new ResolverContext<T> To(ConstructorSelector constructorSelector = null)
        {
            return To<T>(constructorSelector);
        }
    }
}