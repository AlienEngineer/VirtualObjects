using System;
using System.Collections.Generic;
using System.Linq;

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
