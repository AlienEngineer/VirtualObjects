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
    class TranslationConfigurationBuilder : ITranslationConfigurationBuilder
    {
        private readonly Func<Attribute, Boolean> _defaultBooleanGetter;
        private readonly ITranslationConfiguration configuration;


        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationConfigurationBuilder"/> class.
        /// </summary>
        public TranslationConfigurationBuilder()
        {
            configuration = new TranslationConfiguration
            {
                ColumnNameGetters = new List<Func<PropertyInfo, String>>(),
                ColumnKeyGetters = new List<Func<PropertyInfo, Boolean>>(),
                ColumnIdentityGetters = new List<Func<PropertyInfo, Boolean>>(),
                ColumnVersionFieldGetters = new List<Func<PropertyInfo, Boolean>>(),
                ColumnIgnoreGetters = new List<Func<PropertyInfo, Boolean>>(),
                ComputedColumnGetters = new List<Func<PropertyInfo, Boolean>>(),
                EntityNameGetters = new List<Func<Type, String>>(),
                ColumnForeignKeyGetters = new List<Func<PropertyInfo, String>>(),
                ColumnForeignKeyLinksGetters = new List<Func<PropertyInfo, String>>(),
                CollectionFilterGetters = new List<Func<PropertyInfo, String>>()
            };

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
            configuration.EntityNameGetters.Insert(0, nameGetter);
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
            configuration.ColumnNameGetters.Insert(0, nameGetter);
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
                keyGetter = _defaultBooleanGetter;
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
            configuration.ColumnKeyGetters.Insert(0, keyGetter);
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
                keyGetter = _defaultBooleanGetter;
            }

            ColumnIdentity(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(keyGetter).Any();
            });
        }

        public void ColumnIdentity<TAttribute>(Func<TAttribute, Boolean> keyGetter = null,
            Predicate<TAttribute> constraint = null) where TAttribute : Attribute
        {
            
        }

        /// <summary>
        /// Appends a parser to find if a column is an Identity key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnIdentity(Func<PropertyInfo, Boolean> keyGetter)
        {
            configuration.ColumnIdentityGetters.Insert(0, keyGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a Version field attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnVersion<TAttribute>(Func<TAttribute, Boolean> keyGetter = null) where TAttribute : Attribute
        {
            if ( keyGetter == null )
            {
                keyGetter = _defaultBooleanGetter;
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
            configuration.ColumnVersionFieldGetters.Insert(0, keyGetter);
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
                ignoreGetter = _defaultBooleanGetter;
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
            configuration.ColumnIgnoreGetters.Insert(0, ignoreGetter);
        }

        /// <summary>
        /// Appends a parser to find a computed a property.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="computedGetter">The computed getter.</param>
        public void ComputedColumn<TAttribute>(Func<TAttribute, Boolean> computedGetter) where TAttribute : Attribute
        {
            if ( computedGetter == null )
            {
                computedGetter = _defaultBooleanGetter;
            }

            ComputedColumn(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(computedGetter).Any();
            });
        }


        /// <summary>
        /// Appends a parser to find a computed a property.
        /// </summary>
        /// <param name="computedGetter">The computed getter.</param>
        public void ComputedColumn(Func<PropertyInfo, Boolean> computedGetter)
        {
            configuration.ComputedColumnGetters.Insert(0, computedGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKey(Func<PropertyInfo, String> foreignKeyGetter)
        {
            configuration.ColumnForeignKeyGetters.Insert(0, foreignKeyGetter);
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
            configuration.ColumnForeignKeyLinksGetters.Insert(0, foreignKeyGetter);
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

        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="filterGetter">The filter getter.</param>
        public void CollectionFilter(Func<PropertyInfo, String> filterGetter)
        {
            configuration.CollectionFilterGetters.Insert(0, filterGetter);
        }

        /// <summary>
        /// Appends a parser to find the filter based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="filterGetter">The filter getter.</param>
        public void CollectionFilter<TAttribute>(Func<TAttribute, String> filterGetter) where TAttribute : Attribute
        {
            CollectionFilter(prop =>
            {
                var attribute = prop.Attribute<TAttribute>();
                return attribute != null ? filterGetter(attribute) : null;
            });
        }
        #endregion

        /// <summary>
        /// Builds the mapper.
        /// </summary>
        /// <returns></returns>
        public ITranslationConfiguration Build()
        {
            return configuration;
        }

    }
}