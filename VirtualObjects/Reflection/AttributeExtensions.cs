using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VirtualObjects.Reflection
{
    internal static class AttributeExtensions
    {
        /// <summary>
        /// Gets the <see href="Attribute"/>s associated with the <paramref name="provider"/>. The resulting
        /// list of attributes can optionally be filtered by suppliying a list of <paramref name="attributeTypes"/>
        /// to include.
        /// </summary>
        /// <returns>A list of the attributes found on the source element. This value will never be null.</returns>
        public static IList<Attribute> Attributes(this ICustomAttributeProvider provider, params Type[] attributeTypes)
        {
            bool hasTypes = attributeTypes != null && attributeTypes.Length > 0;
            return provider.GetCustomAttributes(true).Cast<Attribute>()
                .Where(attr => !hasTypes ||
                        attributeTypes.Any(at =>
                        {
                            Type type = attr.GetType();
                            return at == type || at.IsSubclassOf(type);
                        })).ToList();
        }
    }
}
