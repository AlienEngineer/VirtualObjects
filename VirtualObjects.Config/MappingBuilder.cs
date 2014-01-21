using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Fasterflect;
using System.Linq;

namespace VirtualObjects.Config
{
    internal interface IMappingBuilder
    {
        IMapper Build();
    }

    public class MappingBuilder : IMappingBuilder
    {
        private readonly ICollection<Func<PropertyInfo, String>> _columnNameGetters;
        private readonly ICollection<Func<PropertyInfo, String>> _columnForeignKeyGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnKeyGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnIdentityGetters;
        private readonly ICollection<Func<Type, String>> _entityNameGetters;
        
        public MappingBuilder()
        {
            _columnNameGetters = new Collection<Func<PropertyInfo, String>>();
            _columnKeyGetters = new Collection<Func<PropertyInfo, Boolean>>();
            _columnIdentityGetters = new Collection<Func<PropertyInfo, Boolean>>();
            _entityNameGetters = new Collection<Func<Type, String>>();
            _columnForeignKeyGetters = new Collection<Func<PropertyInfo, string>>();
        }

        #region Building Methods

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
                return attribute != null ? nameGetter(attribute) : null;
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
                return attribute != null && keyGetter(attribute);
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
                return attribute != null && keyGetter(attribute);
            });
        }

        public void ColumnIdentityFromProperty(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnIdentityGetters.Add(keyGetter);
        }

        public void ForeignKeyFromProperty(Func<PropertyInfo, String> foreignKeyGetter)
        {
            _columnForeignKeyGetters.Add(foreignKeyGetter);
        }

        public void ForeignKeyFromAttribute<TAttribute>(Func<TAttribute, String> foreignKeyGetter) where TAttribute : Attribute
        {
            ForeignKeyFromProperty(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null ? foreignKeyGetter(attribute) : null;
            });
        }

        #endregion
        
        public IMapper Build()
        {
            return new Mapper
            {
                ColumnNameGetters = _columnNameGetters.Reverse(),
                EntityNameGetters = _entityNameGetters.Reverse(),
                ColumnIdentityGetters = _columnIdentityGetters.Reverse(),
                ColumnKeyGetters = _columnKeyGetters.Reverse(),
                ColumnForeignKey = _columnForeignKeyGetters.Reverse()
            };
        }

    }
}