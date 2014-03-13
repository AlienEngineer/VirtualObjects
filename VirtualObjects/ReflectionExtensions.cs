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

        public static Boolean IsVirtual(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod.IsVirtual;
        }

        public static Boolean IsCollection(this Type type)
        {
            return type.GetInterfaces()
                .Any(e => e == typeof(IEnumerable));
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
