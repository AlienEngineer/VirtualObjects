using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionExtensions
    {

        /// <summary>
        /// Determines whether the specified type is dynamic.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Boolean IsDynamic(this Type type)
        {
            return type.Name.StartsWith("<>");
        }

        /// <summary>
        /// Determines whether the specified type is proxy.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Boolean IsProxy(this Type type)
        {
            return type.Name.EndsWith("Proxy") && type.BaseType != null && type.Name.StartsWith(type.BaseType.Name);
        }

        /// <summary>
        /// Determines whether the specified property information is virtual.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        public static Boolean IsVirtual(this PropertyInfo propertyInfo)
        {
#if NET40
            return propertyInfo.GetGetMethod().IsVirtual;
#else
            return propertyInfo.GetMethod.IsVirtual;
#endif
        }

        /// <summary>
        /// Determines whether the specified type is collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Boolean IsCollection(this Type type)
        {
            return type.GetInterfaces()
                .Any(e => e == typeof(IEnumerable))
                && type != typeof(String);
        }


        public static Boolean IsType(this Type type, Type target)
        {
            return type.GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == target);
        }

        /// <summary>
        /// Determines whether [the specified type] [is generic collection] .
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Boolean IsGenericCollection(this Type type)
        {
            return type.IsCollection() && type.GetGenericArguments().Any();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public static class IEnumerableExtenstions
    {
        /// <summary>
        /// Iterates a collection calling the action for each element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var forEach = collection as IList<T> ?? collection.ToList();

            foreach (var item in forEach)
            {
                action(item);
            }

            return forEach;
        } 
    }
}
