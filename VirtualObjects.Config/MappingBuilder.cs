using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;
using System.Linq;

namespace VirtualObjects.Config
{
    class MappingBuilder
    {
        private readonly ICollection<Func<PropertyInfo, String>> _columnNameGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnKeyGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnIdentityGetters;
        private readonly ICollection<Func<Type, String>> _entityNameGetters;
        
        public MappingBuilder()
        {
            _columnNameGetters = new List<Func<PropertyInfo, String>>();
            _columnKeyGetters = new List<Func<PropertyInfo, Boolean>>();
            _columnIdentityGetters = new List<Func<PropertyInfo, Boolean>>();
            _entityNameGetters = new List<Func<Type, String>>();
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
            _entityNameGetters.Add(nameGetter);
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
            _columnNameGetters.Add(nameGetter);
        }

        public void ColumnKeyFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter) where TAttribute : Attribute
        {
            ColumnKeyFromProperty(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null ? keyGetter(attribute) : false;
            });
        }

        public void ColumnKeyFromProperty(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnKeyGetters.Add(keyGetter);
        }

        public void ColumnIdentityFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter) where TAttribute : Attribute
        {
            ColumnIdentityFromProperty(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null ? keyGetter(attribute) : false;
            });
        }

        public void ColumnIdentityFromProperty(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnIdentityGetters.Add(keyGetter);
        }
        
        public IMapper Build()
        {
            return new Mapper
            {
                ColumnNameGetters = _columnNameGetters.Reverse(),
                EntityNameGetters = _entityNameGetters.Reverse(),
                ColumnIdentityGetters = _columnIdentityGetters.Reverse(),
                ColumnKeyGetters = _columnKeyGetters.Reverse()
            };
        }
    }
}