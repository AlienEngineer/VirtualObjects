using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;
using System.Linq;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class MappingBuilder
    {
        private readonly ICollection<Func<PropertyInfo, String>> nameFromPropertyGetters;
        
        public MappingBuilder()
        {
            nameFromPropertyGetters = new List<Func<PropertyInfo, String>>();
        }

        /// <summary>
        /// Names from attribute.
        /// </summary>
        /// <typeparam name="T">The type of the T.</typeparam>
        /// <param name="e">The e.</param>
        public void NameFromAttribute<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute
        {
            NameFromProperty(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null  ? nameGetter(attribute) : null;
            });
        }

        /// <summary>
        /// Names from property.
        /// </summary>
        /// <param name="e">The e.</param>
        public void NameFromProperty(Func<PropertyInfo, String> nameGetter)
        {
            nameFromPropertyGetters.Add(nameGetter);
        }

        public IMapper Build()
        {
            return new Mapper
            {
                NameFromPropertyGetters = nameFromPropertyGetters.Reverse()
            };
        }
    }
}