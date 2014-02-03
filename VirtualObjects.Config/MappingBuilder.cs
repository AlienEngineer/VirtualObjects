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
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnVersionGetters;
        private readonly ICollection<Func<Type, String>> _entityNameGetters;

        private Func<Attribute, Boolean> _defaultBooleanGetter;

        public MappingBuilder()
        {
            _columnNameGetters = new Collection<Func<PropertyInfo, String>>();
            _columnKeyGetters = new Collection<Func<PropertyInfo, Boolean>>();
            _columnIdentityGetters = new Collection<Func<PropertyInfo, Boolean>>();
            _columnVersionGetters = new Collection<Func<PropertyInfo, Boolean>>();
            _entityNameGetters = new Collection<Func<Type, String>>();
            _columnForeignKeyGetters = new Collection<Func<PropertyInfo, string>>();

            _defaultBooleanGetter = attribute => attribute != null;
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

        public void ColumnKeyFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if ( keyGetter == null )
            {
                keyGetter = _defaultBooleanGetter;
            }

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

        public void ColumnIdentityFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if ( keyGetter == null )
            {
                keyGetter = _defaultBooleanGetter;
            }

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

        public void ColumnVersionFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if (keyGetter == null)
            {
                keyGetter = _defaultBooleanGetter;
            }

            ColumnVersionFromProperty(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null && keyGetter(attribute);
            });
        }

        public void ColumnVersionFromProperty(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnVersionGetters.Add(keyGetter);
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
                ColumnForeignKey = _columnForeignKeyGetters.Reverse(),
                ColumnVersionField = _columnVersionGetters.Reverse()
            };
        }

    }
}