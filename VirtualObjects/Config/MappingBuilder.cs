using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Fasterflect;
using System.Linq;
using VirtualObjects.Queries;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMappingBuilder
    {
        /// <summary>
        /// Builds the mapper.
        /// </summary>
        /// <returns></returns>
        IMapper Build();

        /// <summary>
        /// Appends a parser to get the name of the entity attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
        void EntityName<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to get the name of the entity based on the type.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        void EntityName(Func<Type, String> nameGetter);

        /// <summary>
        /// Appends a parser to get the name of the column attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
        void ColumnName<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to get the name of the column based on a Property.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        void ColumnName(Func<PropertyInfo, String> nameGetter);

        /// <summary>
        /// Appends a parser to find if a column is a key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnKey<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to find if a column is a key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnKey(Func<PropertyInfo, Boolean> keyGetter);
        
        /// <summary>
        /// Appends a parser to find if a column is a Identity key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnIdentity<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to find if a column is an Identity key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnIdentity(Func<PropertyInfo, Boolean> keyGetter);
        
        /// <summary>
        /// Appends a parser to find if a column is a Version field attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnVersion<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to find if a column is a Version field based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnVersion(Func<PropertyInfo, Boolean> keyGetter);
        
        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        void ForeignKey(Func<PropertyInfo, String> foreignKeyGetter);
                
        /// <summary>
        /// Appends a parser to find the association based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        void ForeignKey<TAttribute>(Func<TAttribute, String> foreignKeyGetter) where TAttribute : Attribute;

        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        void ForeignKeyLinks(Func<PropertyInfo, String> foreignKeyGetter);

        /// <summary>
        /// Appends a parser to find the association based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        void ForeignKeyLinks<TAttribute>(Func<TAttribute, String> foreignKeyGetter) where TAttribute : Attribute;

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="ignoreGetter">The ignore getter.</param>
        void ColumnIgnore<TAttribute>(Func<TAttribute, Boolean> ignoreGetter = null) where TAttribute : Attribute;

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <param name="ignoreGetter">The ignore getter.</param>
        void ColumnIgnore(Func<PropertyInfo, Boolean> ignoreGetter);
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class MappingBuilder : IMappingBuilder
    {
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnIgnoreGetters;
        private readonly IOperationsProvider _operationsProvider;
        private readonly ICollection<Func<PropertyInfo, String>> _columnNameGetters;
        private readonly ICollection<Func<PropertyInfo, String>> _columnForeignKeyGetters;
        private readonly ICollection<Func<PropertyInfo, String>> _columnForeignKeyLinksGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnKeyGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnIdentityGetters;
        private readonly ICollection<Func<PropertyInfo, Boolean>> _columnVersionGetters;
        private readonly ICollection<Func<Type, String>> _entityNameGetters;

        private readonly Func<Attribute, Boolean> _defaultBooleanGetter;
        private readonly IEntityProvider _entityProvider;
        private readonly IEntityMapper _entityMapper;
        private readonly SessionContext _sessionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingBuilder"/> class.
        /// </summary>
        /// <param name="operationsProvider">The operations provider.</param>
        /// <param name="entityProvider">The entity provider.</param>
        /// <param name="entityMapper">The entity mapper.</param>
        /// <param name="sessionContext">The session context.</param>
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
            _columnIgnoreGetters = new Collection<Func<PropertyInfo, Boolean>>();
            _entityNameGetters = new Collection<Func<Type, String>>();
            _columnForeignKeyGetters = new Collection<Func<PropertyInfo, String>>();
            _columnForeignKeyLinksGetters = new Collection<Func<PropertyInfo, String>>();

            _defaultBooleanGetter = attribute => attribute != null;
        }

        #region Building Methods

        /// <summary>
        /// Appends a parser to get the name of the entity attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
        public void EntityName<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute
        {
            EntityName(type =>
            {
                var attributes = type.Attributes<TAttribute>();

                return attributes != null ?
                    attributes.Select(nameGetter).FirstOrDefault(e => !String.IsNullOrEmpty(e))
                    : null;
            });
        }

        /// <summary>
        /// Appends a parser to get the name of the entity based on the type.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        public void EntityName(Func<Type, String> nameGetter)
        {
            _entityNameGetters.Add(nameGetter);
        }

        /// <summary>
        /// Appends a parser to get the name of the column attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
        public void ColumnName<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute
        {
            ColumnName(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null ?
                    attributes.Select(nameGetter).FirstOrDefault(e => !String.IsNullOrEmpty(e)) 
                    : null;
            });
        }

        /// <summary>
        /// Appends a parser to get the name of the column based on a Property.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        public void ColumnName(Func<PropertyInfo, String> nameGetter)
        {
            _columnNameGetters.Add(nameGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnKey<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if ( keyGetter == null )
            {
#if NET35
                keyGetter = (Func<TAttribute, Boolean>)((TAttribute attribute) => attribute != null);
#else
                keyGetter = _defaultBooleanGetter;
#endif
            }

            ColumnKey(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
            });
        }

        /// <summary>
        /// Appends a parser to find if a column is a key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnKey(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnKeyGetters.Add(keyGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a Identity key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnIdentity<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if ( keyGetter == null )
            {
#if NET35
                keyGetter = (Func<TAttribute, Boolean>)((TAttribute attribute) => attribute != null);
#else
                keyGetter = _defaultBooleanGetter;
#endif
            }

            ColumnIdentity(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
            });
        }

        /// <summary>
        /// Appends a parser to find if a column is an Identity key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnIdentity(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnIdentityGetters.Add(keyGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a Version field attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnVersion<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if (keyGetter == null)
            {
#if NET35
                keyGetter = (Func<TAttribute, Boolean>)((TAttribute attribute) => attribute != null);
#else
                keyGetter = _defaultBooleanGetter;
#endif
            }

            ColumnVersion(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
            });
        }

        /// <summary>
        /// Appends a parser to find if a column is a Version field based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnVersion(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnVersionGetters.Add(keyGetter);
        }

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="ignoreGetter">The ignore getter.</param>
        public void ColumnIgnore<TAttribute>(Func<TAttribute, Boolean> ignoreGetter) where TAttribute : Attribute
        {
            if ( ignoreGetter == null )
            {
#if NET35
                ignoreGetter = (Func<TAttribute, Boolean>)((TAttribute attribute) => attribute != null);
#else
                ignoreGetter = _defaultBooleanGetter;
#endif
            }

            ColumnIgnore(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(ignoreGetter).Any();
            });
        }

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <param name="ignoreGetter">The ignore getter.</param>
        public void ColumnIgnore(Func<PropertyInfo, Boolean> ignoreGetter)
        {
            _columnIgnoreGetters.Add(ignoreGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKey(Func<PropertyInfo, String> foreignKeyGetter)
        {
            _columnForeignKeyGetters.Add(foreignKeyGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKey<TAttribute>(Func<TAttribute, String> foreignKeyGetter) where TAttribute : Attribute
        {
            ForeignKey(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null ? foreignKeyGetter(attribute) : null;
            });
        }

        
        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKeyLinks(Func<PropertyInfo, String> foreignKeyGetter)
        {
            _columnForeignKeyLinksGetters.Add(foreignKeyGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKeyLinks<TAttribute>(Func<TAttribute, String> foreignKeyGetter) where TAttribute : Attribute
        {
            ForeignKeyLinks(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null ? foreignKeyGetter(attribute) : null;
            });
        }
        
        #endregion

        /// <summary>
        /// Builds the mapper.
        /// </summary>
        /// <returns></returns>
        public IMapper Build()
        {
            return new Mapper(_operationsProvider, _entityProvider, _entityMapper, _sessionContext)
            {
                ColumnNameGetters = _columnNameGetters.Reverse(),
                EntityNameGetters = _entityNameGetters.Reverse(),
                ColumnIdentityGetters = _columnIdentityGetters.Reverse(),
                ColumnKeyGetters = _columnKeyGetters.Reverse(),
                ColumnForeignKey = _columnForeignKeyGetters.Reverse(),
                ColumnForeignKeyLinks = _columnForeignKeyLinksGetters.Reverse(),
                ColumnVersionField = _columnVersionGetters.Reverse(),
                ColumnIgnoreGetters = _columnIgnoreGetters.Reverse()
            };
        }

    }
}