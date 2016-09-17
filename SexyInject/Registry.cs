using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SexyInject.Emit;

namespace SexyInject
{
    public class Registry
    {
        private static readonly MethodInfo getMethod = typeof(Registry).GetMethods().Single(x => x.Name == nameof(Get) && x.GetParameters().Length == 2 && x.GetParameters()[1].ParameterType == typeof(Argument[]));
        private static readonly MethodInfo partialApplicationMethod = typeof(Registry).GetMethods().Single(x => x.Name == nameof(Construct) && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.IsGenericType && x.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));
        private readonly ConcurrentDictionary<Type, Binding> bindings = new ConcurrentDictionary<Type, Binding>();
        private readonly ConcurrentDictionary<Tuple<Type, ConstructorSelector>, Func<ResolveContext, object>> factoryCache = new ConcurrentDictionary<Tuple<Type, ConstructorSelector>, Func<ResolveContext, object>>();
        private readonly ConcurrentQueue<IGlobalResolverOperator> globalOperators = new ConcurrentQueue<IGlobalResolverOperator>();

        public IEnumerable<IGlobalResolverOperator> GlobalOperators => globalOperators;

        public void Bind<T>(Func<Binder<T>, ResolverContext> binder = null)
        {
            binder = binder ?? (x => x.To<T>());
            var binding = bindings.GetOrAdd(typeof(T), x => new Binding(this, x));
            var context = binder(new Binder<T>(binding));
            context.Close();
        }

        public void Bind(Type type, Func<Binder, ResolverContext> binder = null)
        {
            binder = binder ?? (x => x.To(type));
            var binding = bindings.GetOrAdd(type, x => new Binding(this, x));
            var context = binder(new Binder(binding));
            context.Close();
        }

        public T Get<T>()
        {
            return Get<T>(new Argument[0]);
        }

        public object Get(Type type)
        {
            return Get(type, new Argument[0]);
        }

        public T Get<T>(params Argument[] arguments)
        {
            return (T)Get(typeof(T), arguments);
        }

        public T Get<T>(params object[] arguments)
        {
            return (T)Get(typeof(T), arguments.ToArguments(ArgumentType.Pooled));
        }

        public object Get(Type type, params object[] arguments)
        {
            return Get(type, arguments.ToArguments(ArgumentType.Pooled));
        }

        public object Get(Type type, params Argument[] arguments)
        {
            return Get(CreateResolverContext(arguments.Where(x => x.ArgumentType == ArgumentType.Pooled).Select(x => x.Value)), type, arguments.Where(x => x.ArgumentType == ArgumentType.Unpooled).Select(x => x.Value).ToArray());
        }

        public Expression GetExpression(Type type)
        {
            return Expression.Convert(Expression.Call(Expression.Constant(this), getMethod, Expression.Constant(type), Expression.Constant(new Argument[0])), type);
        }

        public Expression PartialApplicationExpression(Type type, Expression constructor)
        {
            return Expression.Call(Expression.Constant(this), partialApplicationMethod.MakeGenericMethod(type), constructor);
        }

        public T Construct<T>()
        {
            return (T)Construct(typeof(T), null, new Argument[0]);
        }

        public T Construct<T>(ConstructorSelector constructorSelector)
        {
            return (T)Construct(typeof(T), constructorSelector, new Argument[0]);
        }

        public T Construct<T>(params Argument[] arguments)
        {
            return (T)Construct(typeof(T), null, arguments);
        }

        public T Construct<T>(params object[] arguments)
        {
            return (T)Construct(typeof(T), null, arguments);
        }

        public T Construct<T>(ConstructorSelector constructorSelector, params Argument[] arguments)
        {
            return (T)Construct(typeof(T), constructorSelector, arguments);
        }

        public T Construct<T>(ConstructorSelector constructorSelector, params object[] arguments)
        {
            return (T)Construct(typeof(T), constructorSelector, arguments);
        }

        public object Construct(Type type)
        {
            return Construct(type, null, new Argument[0]);
        }

        public object Construct(Type type, params Argument[] arguments)
        {
            return Construct(type, null, arguments);
        }

        public object Construct(Type type, params object[] arguments)
        {
            return Construct(type, null, arguments.ToArguments(ArgumentType.Pooled));
        }

        public object Construct(Type type, ConstructorSelector constructorSelector, params object[] arguments)
        {
            return Construct(type, constructorSelector, arguments.ToArguments(ArgumentType.Pooled));
        }

        public object Construct(Type type, ConstructorSelector constructorSelector, params Argument[] arguments)
        {
            return Construct(CreateResolverContext(arguments.Where(x => x.ArgumentType == ArgumentType.Pooled).Select(x => x.Value)), type, constructorSelector, arguments.Where(x => x.ArgumentType == ArgumentType.Unpooled).Select(x => x.Value).ToArray());
        }

        public T Construct<T>(Func<ResolveContext, T> constructor)
        {
            var context = CreateResolverContext(Enumerable.Empty<object>());
            return context.Construct(constructor);
        }

        public object Construct(Type type, Delegate constructor)
        {
            var context = CreateResolverContext(Enumerable.Empty<object>());
            return context.Construct(type, constructor);
        }

        public void AddGlobalHeadOperator(Func<ResolverContext, ResolverContext> @operator, Func<ResolverContext, bool> predicate = null)
        {
            AddGlobalOperator(new LambdaGlobalResolverOperator(headOperators: @operator, headOperatorPredicate: predicate));
        }

        public void AddGlobalTailOperator(Func<ResolverContext, ResolverContext> @operator, Func<ResolverContext, bool> predicate = null)
        {
            AddGlobalOperator(new LambdaGlobalResolverOperator(tailOperators: @operator, tailOperatorPredicate: predicate));
        }

        public void AddGlobalOperator(IGlobalResolverOperator @operator)
        {
            globalOperators.Enqueue(@operator);
            foreach (var binding in bindings.Values)
            {
                foreach (var context in binding.ResolverContexts)
                {
                    context.AddGlobalOperator(@operator);
                }
            }
        }

        private object Construct(ResolveContext context, Type type, ConstructorSelector constructorSelector, object[] arguments)
        {
            var result = context.Construct(type, constructorSelector, arguments);
            return result;
        }

        private ResolveContext CreateResolverContext(IEnumerable<object> arguments)
        {
            ResolveContext context = null;
            context = new ResolveContext(
                this,
                x => Resolve(context, x),
                (type, constructorSelector) => factoryCache.GetOrAdd(Tuple.Create(type, constructorSelector), x => FactoryGenerator(x.Item1, x.Item2))(context),
                arguments);
            return context;
        }

        private object Resolve(ResolveContext context, Type type)
        {
            Binding binding = null;
            foreach (var current in context.EnumerateTypeHierarchy(type))
            {
                if (bindings.TryGetValue(current, out binding))
                    break;
            }
            if (binding == null)
                throw new RegistryException(new[] { type }, $"The type {type.FullName} has not been registered.");

            return binding.Resolve(context, type);
        }

        private object Get(ResolveContext context, Type type, object[] arguments)
        {
            var result = context.Resolve(type, arguments);
            return result;
        }

        private Func<ResolveContext, object> FactoryGenerator(Type type, ConstructorSelector constructorSelector)
        {
            constructorSelector = constructorSelector ?? (constructors => constructors.OrderByDescending(x => x.GetParameters().Length).FirstOrDefault());

            var constructor = constructorSelector(type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public));
            var parameters = constructor.GetParameters();
            var contextParameter = Expression.Parameter(typeof(ResolveContext), "context");

            // context.TryResolve(arg0Type), context.TryResolve(arg1Type)...
            var arguments = parameters.Select(x => Expression.Convert(ResolveContext.ResolveExpression(contextParameter, x.ParameterType, Expression.Constant(new object[0])), x.ParameterType)).ToArray();

            // new T(arguments)
            Expression body = Expression.New(constructor, arguments);

            if (type.IsValueType)
                body = Expression.Convert(body, typeof(object));

            // context => body
            var lambda = Expression.Lambda<Func<ResolveContext, object>>(body, contextParameter);

            // Compile it into a delegate we can actually invoke
            return lambda.Compile();
        }
    }
}