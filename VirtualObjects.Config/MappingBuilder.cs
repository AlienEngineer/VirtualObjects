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
        private readonly ICollection<Func<PropertyInfo, String>> columnNameGetters;
        private readonly ICollection<Func<Type, String>> entityNameGetters;
        
        public MappingBuilder()
        {
            columnNameGetters = new List<Func<PropertyInfo, String>>();
            entityNameGetters = new List<Func<Type, String>>();
        }

        public void EntityNameFromAttribute<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute
        {
            EntityNameFromType(type =>
            {
                var attribute = type.Attribute<TAttribute>();
                return attribute != null ? nameGetter(attribute) : null;
            });
        }

        public void EntityNameFromType(Func<Type, String> nameGetter)
        {
            entityNameGetters.Add(nameGetter);
        }

        public void ColumnNameFromAttribute<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute
        {
            ColumnNameFromProperty(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null  ? nameGetter(attribute) : null;
            });
        }

        public void ColumnNameFromProperty(Func<PropertyInfo, String> nameGetter)
        {
            columnNameGetters.Add(nameGetter);
        }

        public IMapper Build()
        {
            return new Mapper
            {
                ColumnNameGetters = columnNameGetters.Reverse(),
                EntityNameGetters = entityNameGetters.Reverse()
            };
        }
    }
}