﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SexyInject
{
    /// <summary>
    /// Helper class to facilitate a fluent syntax for customizing the binding between a specified pair of types.
    /// </summary>
    public class ResolverContext
    {
        public Registry Registry { get; }
        public Binding Binding { get; }
        public IResolver Resolver { get; }
        public IReadOnlyList<IResolverOperator> Operators => operators;

        private readonly List<IResolverOperator> operators = new List<IResolverOperator>();

        public ResolverContext(Registry registry, Binding binding, IResolver resolver)
        {
            Registry = registry;
            Binding = binding;
            Resolver = resolver;
        }

        private void SpliceHeadOperators(Action action)
        {
            var currentOperatorCount = operators.Count;
            action();
            var newOperatorsCount = operators.Count - currentOperatorCount;
            if (newOperatorsCount > 0)
            {
                var newOperators = new IResolverOperator[newOperatorsCount];
                for (int i = currentOperatorCount, j = 0; i < operators.Count; i++, j++)
                    newOperators[j] = operators[i];
                operators.RemoveRange(currentOperatorCount, newOperatorsCount);
                operators.InsertRange(0, newOperators);
            }
        }

        internal void Close()
        {
            SpliceHeadOperators(() =>
            {
                foreach (var globalOperator in Registry.GlobalOperators)
                {
                    globalOperator.AddHeadOperators(this);
                }
            });
            foreach (var globalOperator in Registry.GlobalOperators)
            {
                globalOperator.AddTailOperators(this);
            }
        }

        internal void AddGlobalOperator(IGlobalResolverOperator @operator)
        {
            SpliceHeadOperators(() => @operator.AddHeadOperators(this));
            @operator.AddTailOperators(this);
        }

        private ResolverProcessor GetResolverProcessor()
        {
            var executed = false;
            return (ResolveContext context, Type type, out object result) =>
            {
                if (executed)
                    throw new InvalidOperationException("An operator may only call the resolver processor one time.");
                executed = true;

                return Resolver.TryResolve(context, type, out result);
            };
        }

        private ResolverProcessor GetOperatorProcessor(int index)
        {
            var executed = false;
            return (ResolveContext context, Type type, out object result) =>
            {
                if (executed)
                    throw new InvalidOperationException("An operator may only call the resolver processor one time.");
                executed = true;

                ResolverProcessor nextProcessor = index > 0 ? GetOperatorProcessor(index - 1) : GetResolverProcessor();
                var @operator = Operators[index];
                return @operator.TryResolve(context, type, nextProcessor, out result);
            };
        }

        public ResolverProcessor ResolverProcessor => Operators.Any() ? GetOperatorProcessor(Operators.Count - 1) : GetResolverProcessor();

        /// <summary>
        /// Constrains the resolver to only be applied when the specified predicate is satisfied.
        /// </summary>
        /// <param name="predicate">When this predicate returns true for a given resolution request, the resolver will be used.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext When(Func<ResolveContext, Type, bool> predicate) => AddOperator(new PredicatedResolver(predicate));

        /// <summary>
        /// Constrains the resolver to only be applied when the specified predicate is satisfied.
        /// </summary>
        /// <param name="predicate">When this predicate returns true for a given resolution request, the resolver will be used.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext When(Func<Type, bool> predicate) => AddOperator(new PredicatedResolver((context, type) => predicate(type)));

        /// <summary>
        /// Cache values transiently.
        /// </summary>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext Cache(TransientCache cacheType) => AddOperator(new TransientCacheResolver());

        /// <summary>
        /// Cache values returned by the resolver based on the key generated by the specified keyGenerator.
        /// </summary>
        /// <param name="keySelector">The result of this selector will be used as the key in the dictionary cache.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext Cache(Func<ResolveContext, Type, object> keySelector) => AddOperator(new CacheResolver(keySelector));

        /// <summary>
        /// Cache values returned by the resolver based on the key generated by the specified keyGenerator.
        /// </summary>
        /// <param name="keySelector">The result of this selector will be used as the key in the dictionary cache.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext Cache(Func<Type, object> keySelector) => AddOperator(new CacheResolver((context, type) => keySelector(type)));

        /// <summary>
        /// For the current binding only, uses the instance provided by the specified factory to resolve any dependencies.
        /// </summary>
        /// <param name="factory">Provides an instance of the dependency to inject</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext Inject(Func<ResolveContext, Type, object> factory) => AddOperator(new ClassInjectionResolver(factory));

        /// <summary>
        /// For the current binding only, uses the instance provided by the specified factory to resolve any dependencies.
        /// </summary>
        /// <param name="factory">Provides an instance of the dependency to inject</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext Inject(Func<Type, object> factory) => AddOperator(new ClassInjectionResolver((context, type) => factory(type)));

        /// <summary>
        /// For the current binding only, uses the instance provided by the specified factory to resolve any dependencies.
        /// </summary>
        /// <param name="property">The property to which the dependency will be injected.</param>
        /// <param name="factory">Provides the dependency that should be injected into the specified property.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext InjectProperty(MemberInfo property, Func<ResolveContext, Type, object> factory)
        {
            return AddOperator(new PropertyInjectionResolver(property, (context, type) => factory(context, type)));
        }

        /// <summary>
        /// For the current binding only, uses the instance provided by the specified factory to resolve any dependencies.
        /// </summary>
        /// <param name="property">The property to which the dependency will be injected.</param>
        /// <param name="factory">Provides the dependency that should be injected into the specified property.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext InjectProperty(MemberInfo property, Func<ResolveContext, object> factory)
        {
            return AddOperator(new PropertyInjectionResolver(property, (context, type) => factory(context)));
        }

        /// <summary>
        /// Makes it so the specified property will be injected with an instance of its type.
        /// </summary>
        /// <param name="property">The property to which the dependency will be injected.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext InjectProperty(MemberInfo property)
        {
            return AddOperator(new PropertyInjectionResolver(property, (context, type) => context.Resolve(type)));
        }

        /// <summary>
        /// Exactly the same as calling the InjectProperty overload that only accepts a property expression,
        /// but for each property you provide in the properties array.
        /// </summary>
        /// <param name="properties">The array of properties that should be injected with dependencies.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext InjectProperties(params MemberInfo[] properties)
        {
            foreach (var property in properties)
                InjectProperty(property);
            return this;
        }

        /// <summary>
        /// Exactly the same as calling the InjectProperties overload that accepts an array of properties except
        /// that that array is pre-populated with, by default, all the writable public properties of T.  You can
        /// provide an alternate filter via predicate.
        /// </summary>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext InjectProperties(Func<PropertyInfo, bool> filter = null)
        {
            filter = filter ?? (property => property.CanWrite && property.SetMethod.IsPublic);
            var properties = Binding.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(filter).ToArray();
            InjectProperties(properties);
            return this;
        }

        /// <summary>
        /// Allows you to provide custom behavior onto an instance after it has been resolved.  Note that order here is
        /// important.  For example, if you apply the Cache operator after the WhenResolved operator, WhenResolved will
        /// only be invoked when the object is first realized.  Conversely, if you apply this operator after the Cache
        /// operator, it will be invoked every time.
        /// </summary>
        /// <param name="handler">With the object in hand, allows you to perform some arbitrary behavior on it.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext WhenResolved(Action<ResolveContext, object> handler)
        {
            return AddOperator(new WhenResolvedResolver(handler));
        }

        /// <summary>
        /// Allows you to provide custom behavior onto an instance after it has been resolved.  Note that order here is
        /// important.  For example, if you apply the Cache operator after the WhenResolved operator, WhenResolved will
        /// only be invoked when the object is first realized.  Conversely, if you apply this operator after the Cache
        /// operator, it will be invoked every time.
        /// </summary>
        /// <param name="handler">With the object in hand, allows you to perform some arbitrary behavior on it.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext WhenResolved(Action<object> handler)
        {
            return AddOperator(new WhenResolvedResolver((context, o) => handler(o)));
        }

        public ResolverContext AddOperator(IResolverOperator @operator)
        {
            operators.Add(@operator);
            return this;
        }
    }

    /// <summary>
    /// Helper class to facilitate a fluent syntax for customizing the binding between a specified pair of types.
    /// </summary>
    /// <typeparam name="T">The type to which the binding will resolve.</typeparam>
    public class ResolverContext<T> : ResolverContext
    {
        public ResolverContext(Registry registry, Binding binding, IResolver resolver) : base(registry, binding, resolver)
        {
        }

        /// <summary>
        /// For the current binding only, uses the instance provided by the specified factory to resolve any dependencies.
        /// </summary>
        /// <typeparam name="TValue">The type of object that is the dependency being injected</typeparam>
        /// <param name="property">The property to which the dependency will be injected.</param>
        /// <param name="factory">Provides the dependency that should be injected into the specified property.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext<T> InjectProperty<TValue>(Expression<Func<T, TValue>> property, Func<ResolveContext, Type, TValue> factory)
        {
            return AddOperator(new PropertyInjectionResolver(property, (context, type) => factory(context, type)));
        }

        /// <summary>
        /// For the current binding only, uses the instance provided by the specified factory to resolve any dependencies.
        /// </summary>
        /// <typeparam name="TValue">The type of object that is the dependency being injected</typeparam>
        /// <param name="property">The property to which the dependency will be injected.</param>
        /// <param name="factory">Provides the dependency that should be injected into the specified property.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext<T> InjectProperty<TValue>(Expression<Func<T, TValue>> property, Func<ResolveContext, TValue> factory)
        {
            return AddOperator(new PropertyInjectionResolver(property, (context, type) => factory(context)));
        }

        /// <summary>
        /// Makes it so the specified property will be injected with an instance of its type.
        /// </summary>
        /// <typeparam name="TValue">The type of object that is the dependency being injected</typeparam>
        /// <param name="property">The property to which the dependency will be injected.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext<T> InjectProperty<TValue>(Expression<Func<T, TValue>> property)
        {
            return AddOperator(new PropertyInjectionResolver(property, (context, type) => context.Resolve(type)));
        }

        /// <summary>
        /// Exactly the same as calling the InjectProperty overload that only accepts a property expression,
        /// but for each property you provide in the properties array.
        /// </summary>
        /// <param name="properties">The array of properties that should be injected with dependencies.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext<T> InjectProperties(params Expression<Func<T, object>>[] properties)
        {
            foreach (var property in properties)
                InjectProperty(property);
            return this;
        }

        /// <summary>
        /// Exactly the same as calling the InjectProperties overload that accepts an array of properties except
        /// that that array is pre-populated with, by default, all the writable public properties of T.  You can
        /// provide an alternate filter via predicate.
        /// </summary>
        /// <returns>This context to facilitate fluent syntax</returns>
        public new ResolverContext<T> InjectProperties(Func<PropertyInfo, bool> filter = null)
        {
            base.InjectProperties(filter);
            return this;
        }

        /// <summary>
        /// Constrains the resolver to only be applied when the specified predicate is satisfied.
        /// </summary>
        /// <param name="predicate">When this predicate returns true for a given resolution request, the resolver will be used.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public new ResolverContext<T> When(Func<ResolveContext, Type, bool> predicate) => AddOperator(new PredicatedResolver(predicate));

        /// <summary>
        /// Constrains the resolver to only be applied when the specified predicate is satisfied.
        /// </summary>
        /// <param name="predicate">When this predicate returns true for a given resolution request, the resolver will be used.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public new ResolverContext<T> When(Func<Type, bool> predicate) => AddOperator(new PredicatedResolver((context, type) => predicate(type)));


        /// <summary>
        /// Cache values returned by the resolver based on the key generated by the specified keyGenerator.
        /// </summary>
        /// <param name="keySelector">The result of this selector will be used as the key in the dictionary cache.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public new ResolverContext<T> Cache(Func<ResolveContext, Type, object> keySelector) => AddOperator(new CacheResolver(keySelector));

        /// <summary>
        /// Cache values returned by the resolver based on the key generated by the specified keyGenerator.
        /// </summary>
        /// <param name="keySelector">The result of this selector will be used as the key in the dictionary cache.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public new ResolverContext<T> Cache(Func<Type, object> keySelector) => AddOperator(new CacheResolver((context, type) => keySelector(type)));

        /// <summary>
        /// Allows you to provide custom behavior onto an instance after it has been resolved.  Note that order here is
        /// important.  For example, if you apply the Cache operator after the WhenResolved operator, WhenResolved will
        /// only be invoked when the object is first realized.  Conversely, if you apply this operator after the Cache
        /// operator, it will be invoked every time.
        /// </summary>
        /// <param name="handler">With the object in hand, allows you to perform some arbitrary behavior on it.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext<T> WhenResolved(Action<ResolveContext, T> handler)
        {
            return AddOperator(new WhenResolvedResolver((context, o) => handler(context, (T)o)));
        }

        /// <summary>
        /// Allows you to provide custom behavior onto an instance after it has been resolved.  Note that order here is
        /// important.  For example, if you apply the Cache operator after the WhenResolved operator, WhenResolved will
        /// only be invoked when the object is first realized.  Conversely, if you apply this operator after the Cache
        /// operator, it will be invoked every time.
        /// </summary>
        /// <param name="handler">With the object in hand, allows you to perform some arbitrary behavior on it.</param>
        /// <returns>This context to facilitate fluent syntax</returns>
        public ResolverContext WhenResolved(Action<T> handler)
        {
            return AddOperator(new WhenResolvedResolver((context, o) => handler((T)o)));
        }

        public new ResolverContext<T> AddOperator(IResolverOperator @operator)
        {
            return (ResolverContext<T>)base.AddOperator(@operator);
        }
    }
}