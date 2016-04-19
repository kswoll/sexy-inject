using System;

namespace SexyInject
{
    /// <summary>
    /// A collection of functions that clarify and consolidate various caching strategies to make them 
    /// easier to read and parsimonious.
    /// </summary>
    public static class Cache
    {
        /// <summary><para>
        /// Caches the instance resolved by the binding based on its type such that the same instance will 
        /// always be returned for a given type that has been requested.  Usage:
        /// </para>
        /// 
        /// <para><c>
        /// .Cache(Cache.Singleton);
        /// </c></para>
        /// </summary>
        public static Func<ResolveContext, Type, object> Singleton { get; } = (context, type) => type;

        /// <summary><para>
        /// Caches the instance resolved by the binding based on its type but only within the scope of a single
        /// request to the registry.  What this means is that if the same type would be injected into multiple
        /// dependencies when trying to obtain an item from the registry, the same instance will be returned, 
        /// rather than a new one for each dependency.
        /// </para>
        /// 
        /// <para><c>
        /// .Cache(Cache.Transient);
        /// </c></para>
        /// </summary>
        public static TransientCache Transient { get; } = new TransientCache();
    }
}