using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects
{
    public static class ReflectionExtensions
    {

        public static Boolean IsDynamic(this Type type)
        {
            return type.Name.StartsWith("<>");
        }

    }

    public static class IEnumerableExtenstions
    {
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
