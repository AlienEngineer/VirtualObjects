using System;
using System.Collections.Generic;

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
        public static  IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
                yield return item;
            }
        } 
    }
}
