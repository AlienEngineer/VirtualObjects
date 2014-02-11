using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Fasterflect;
using System.Linq;
using VirtualObjects.Queries;

namespace VirtualObjects.Config
{
    public interface IMappingBuilder
    {
        IMapper Build();

        void EntityNameFromAttribute<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute;
        void EntityNameFromType(Func<Type, String> nameGetter);
        void ColumnNameFromAttribute<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute;
        void ColumnNameFromProperty(Func<PropertyInfo, String> nameGetter);
        void ColumnKeyFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        void ColumnKeyFromProperty(Func<PropertyInfo, Boolean> keyGetter);
        void ColumnIdentityFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        void ColumnIdentityFromProperty(Func<PropertyInfo, Boolean> keyGetter);
        void ColumnVersionFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        void ColumnVersionFromProperty(Func<PropertyInfo, Boolean> keyGetter);
        void ForeignKeyFromProperty(Func<PropertyInfo, String> foreignKeyGetter);
        void ForeignKeyFromAttribute<TAttribute>(Func<TAttribute, String> foreignKeyGetter) where TAttribute : Attribute;
    }

    public class MappingBuilder : IMappingBuilder
    {
        private readonly IOperationsProvider _operationsProvider;
        private readonly ICollection<Func<PropertyInfo, String>> _columnNameGetters;
        private readonly ICollection<Func<PropertyInfo, String>> _columnForeignKeyGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnKeyGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnIdentityGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnVersionGetters;
        private readonly ICollection<Func<Type, String>> _entityNameGetters;

        private readonly Func<Attribute, Boolean> _defaultBooleanGetter;
        private readonly IEntityProvider _entityProvider;
        private readonly IEntityMapper _entityMapper;
        private readonly SessionContext _sessionContext;

        public MappingBuilder(IOperationsProvider operationsProvider, IEntityProvider entityProvider, IEntityMapper entityMapper, SessionContext sessionContext)
        {
            _operationsProvider = operationsProvider;
            _entityProvider = entityProvider;
            _entityMapper = entityMapper;
            _sessionContext = sessionContext;
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
                var attributes = type.Attributes<TAttribute>();

                return attributes != null ?
                    attributes.Select(nameGetter).FirstOrDefault(e => !String.IsNullOrEmpty(e))
                    : null;
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
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null ?
                    attributes.Select(nameGetter).FirstOrDefault(e => !String.IsNullOrEmpty(e)) 
                    : null;
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
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
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
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
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
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
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
            return new Mapper(_operationsProvider, _entityProvider, _entityMapper, _sessionContext)
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