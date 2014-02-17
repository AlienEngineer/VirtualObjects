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
        void EntityNameFromAttribute<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to get the name of the entity based on the type.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        void EntityNameFromType(Func<Type, String> nameGetter);

        /// <summary>
        /// Appends a parser to get the name of the column attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
        void ColumnNameFromAttribute<TAttribute>(Func<TAttribute, String> nameGetter) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to get the name of the column based on a Property.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        void ColumnNameFromProperty(Func<PropertyInfo, String> nameGetter);

        /// <summary>
        /// Appends a parser to find if a column is a key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnKeyFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to find if a column is a key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnKeyFromProperty(Func<PropertyInfo, Boolean> keyGetter);
        
        /// <summary>
        /// Appends a parser to find if a column is a Identity key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnIdentityFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to find if a column is an Identity key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnIdentityFromProperty(Func<PropertyInfo, Boolean> keyGetter);
        
        /// <summary>
        /// Appends a parser to find if a column is a Version field attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnVersionFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute;
        
        /// <summary>
        /// Appends a parser to find if a column is a Version field based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        void ColumnVersionFromProperty(Func<PropertyInfo, Boolean> keyGetter);
        
        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        void ForeignKeyFromProperty(Func<PropertyInfo, String> foreignKeyGetter);
        
        /// <summary>
        /// Appends a parser to find the association based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        void ForeignKeyFromAttribute<TAttribute>(Func<TAttribute, String> foreignKeyGetter) where TAttribute : Attribute;
    }

    /// <summary>
    /// 
    /// </summary>
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
            _entityNameGetters = new Collection<Func<Type, String>>();
            _columnForeignKeyGetters = new Collection<Func<PropertyInfo, string>>();

            _defaultBooleanGetter = attribute => attribute != null;
        }

        #region Building Methods

        /// <summary>
        /// Appends a parser to get the name of the entity attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
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

        /// <summary>
        /// Appends a parser to get the name of the entity based on the type.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        public void EntityNameFromType(Func<Type, String> nameGetter)
        {
            _entityNameGetters.Add(nameGetter);
        }

        /// <summary>
        /// Appends a parser to get the name of the column attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
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

        /// <summary>
        /// Appends a parser to get the name of the column based on a Property.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        public void ColumnNameFromProperty(Func<PropertyInfo, String> nameGetter)
        {
            _columnNameGetters.Add(nameGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnKeyFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if ( keyGetter == null )
            {
#if NET35
                keyGetter = (Func<TAttribute, Boolean>)((TAttribute attribute) => attribute != null);
#else
                keyGetter = _defaultBooleanGetter;
#endif
            }

            ColumnKeyFromProperty(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
            });
        }

        /// <summary>
        /// Appends a parser to find if a column is a key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnKeyFromProperty(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnKeyGetters.Add(keyGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a Identity key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnIdentityFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if ( keyGetter == null )
            {
#if NET35
                keyGetter = (Func<TAttribute, Boolean>)((TAttribute attribute) => attribute != null);
#else
                keyGetter = _defaultBooleanGetter;
#endif
            }

            ColumnIdentityFromProperty(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
            });
        }

        /// <summary>
        /// Appends a parser to find if a column is an Identity key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnIdentityFromProperty(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnIdentityGetters.Add(keyGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a Version field attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnVersionFromAttribute<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if (keyGetter == null)
            {
#if NET35
                keyGetter = (Func<TAttribute, Boolean>)((TAttribute attribute) => attribute != null);
#else
                keyGetter = _defaultBooleanGetter;
#endif
            }

            ColumnVersionFromProperty(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
            });
        }

        /// <summary>
        /// Appends a parser to find if a column is a Version field based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnVersionFromProperty(Func<PropertyInfo, Boolean> keyGetter)
        {
            _columnVersionGetters.Add(keyGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKeyFromProperty(Func<PropertyInfo, String> foreignKeyGetter)
        {
            _columnForeignKeyGetters.Add(foreignKeyGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKeyFromAttribute<TAttribute>(Func<TAttribute, String> foreignKeyGetter) where TAttribute : Attribute
        {
            ForeignKeyFromProperty(prop =>
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
                ColumnVersionField = _columnVersionGetters.Reverse()
            };
        }

    }
}