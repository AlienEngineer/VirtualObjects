using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace VirtualObjects.Config
{

    /// <summary>
    /// 
    /// </summary>
    class TranslationConfigurationBuilder : ITranslationConfigurationBuilder
    {
        private readonly Func<Attribute, bool> _defaultBooleanGetter;
        private readonly ITranslationConfiguration _configuration;


        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationConfigurationBuilder"/> class.
        /// </summary>
        public TranslationConfigurationBuilder()
        {
            _configuration = new TranslationConfiguration
            {
                ColumnNameGetters = new List<Func<PropertyInfo, string>>(),
                ColumnKeyGetters = new List<Func<PropertyInfo, bool>>(),
                ColumnIdentityGetters = new List<Func<PropertyInfo, bool>>(),
                ColumnVersionFieldGetters = new List<Func<PropertyInfo, bool>>(),
                ColumnIgnoreGetters = new List<Func<PropertyInfo, bool>>(),
                ComputedColumnGetters = new List<Func<PropertyInfo, bool>>(),
                IsForeignKeyGetters = new List<Func<PropertyInfo, bool>>(),
                EntityNameGetters = new List<Func<Type, string>>(),
                ColumnForeignKeyGetters = new List<Func<PropertyInfo, string>>(),
                ColumnForeignKeyLinksGetters = new List<Func<PropertyInfo, string>>(),
                CollectionFilterGetters = new List<Func<PropertyInfo, string>>(),
                ColumnFormattersGetters = new List<Func<PropertyInfo, string>>(),
                ColumnNumberFormattersGetters = new List<Func<PropertyInfo, NumberFormatInfo>>()
            };

            _defaultBooleanGetter = attribute => attribute != null;
        }

        #region Building Methods
        
        /// <summary>
        /// Appends a parser to get the name of the entity attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
        public void EntityName<TAttribute>(Func<TAttribute, string> nameGetter) where TAttribute : Attribute
        {
            EntityName(type =>
            {
                var attributes = type.Attributes<TAttribute>();

                return attributes != null ?
                    attributes.Select(nameGetter).FirstOrDefault(e => !string.IsNullOrEmpty(e))
                    : null;
            });
        }

        /// <summary>
        /// Appends a parser to get the name of the entity based on the type.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        public void EntityName(Func<Type, string> nameGetter)
        {
            _configuration.EntityNameGetters.Insert(0, nameGetter);
        }

        /// <summary>
        /// Appends a parser to get the name of the column attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="nameGetter">The name getter.</param>
        public void ColumnName<TAttribute>(Func<TAttribute, string> nameGetter) where TAttribute : Attribute
        {
            ColumnName(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null ?
                    attributes.Select(nameGetter).FirstOrDefault(e => !string.IsNullOrEmpty(e))
                    : null;
            });
        }

        /// <summary>
        /// Appends a parser to get the name of the column based on a Property.
        /// </summary>
        /// <param name="nameGetter">The name getter.</param>
        public void ColumnName(Func<PropertyInfo, string> nameGetter)
        {
            _configuration.ColumnNameGetters.Insert(0, nameGetter);
        }

        /// <summary>
        /// Appends a parser to get the number format of the column attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="numberFormatGetter">The number format getter.</param>
        public void ColumnNumberFormat<TAttribute>(Func<TAttribute, NumberFormatInfo> numberFormatGetter) where TAttribute : Attribute
        {
            ColumnNumberFormat(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null ?
                    attributes.Select(numberFormatGetter).FirstOrDefault()
                    : null;
            });
        }

        /// <summary>
        /// Appends a parser to get the number format of the column based on a Property.
        /// </summary>
        /// <param name="numberFormatGetter">The number format getter.</param>
        public void ColumnNumberFormat(Func<PropertyInfo, NumberFormatInfo> numberFormatGetter)
        {
            _configuration.ColumnNumberFormattersGetters.Insert(0, numberFormatGetter);
        }

        /// <summary>
        /// Appends a parser to get the format of the column attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="formatGetter">The format getter.</param>
        public void ColumnFormat<TAttribute>(Func<TAttribute, string> formatGetter) where TAttribute : Attribute
        {
            ColumnFormat(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null ?
                    attributes.Select(formatGetter).FirstOrDefault(e => !string.IsNullOrEmpty(e))
                    : null;
            });
        }

        /// <summary>
        /// Appends a parser to get the format of the column based on a Property.
        /// </summary>
        /// <param name="formatGetter">The format getter.</param>
        public void ColumnFormat(Func<PropertyInfo, string> formatGetter)
        {
            _configuration.ColumnFormattersGetters.Insert(0, formatGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnKey<TAttribute>(Func<TAttribute, bool> keyGetter = null) where TAttribute : Attribute
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
        public void ColumnKey(Func<PropertyInfo, bool> keyGetter)
        {
            _configuration.ColumnKeyGetters.Insert(0, keyGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a Identity key attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnIdentity<TAttribute>(Func<TAttribute, bool> keyGetter = null) where TAttribute : Attribute
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

        /// <summary>
        /// Appends a parser to find if a column is an Identity key based on a Property.
        /// </summary>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnIdentity(Func<PropertyInfo, bool> keyGetter)
        {
            _configuration.ColumnIdentityGetters.Insert(0, keyGetter);
        }

        /// <summary>
        /// Appends a parser to find if a column is a Version field attribute based.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="keyGetter">The key getter.</param>
        public void ColumnVersion<TAttribute>(Func<TAttribute, bool> keyGetter = null) where TAttribute : Attribute
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
        public void ColumnVersion(Func<PropertyInfo, bool> keyGetter)
        {
            _configuration.ColumnVersionFieldGetters.Insert(0, keyGetter);
        }

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="ignoreGetter">The ignore getter.</param>
        public void ColumnIgnore<TAttribute>(Func<TAttribute, bool> ignoreGetter) where TAttribute : Attribute
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
        public void ColumnIgnore(Func<PropertyInfo, bool> ignoreGetter)
        {
            _configuration.ColumnIgnoreGetters.Insert(0, ignoreGetter);
        }

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="isForeignKeyGetter">The is foreign key getter.</param>
        public void IsForeignKey<TAttribute>(Func<TAttribute, bool> isForeignKeyGetter) where TAttribute : Attribute
        {
            if (isForeignKeyGetter == null)
            {
                isForeignKeyGetter = _defaultBooleanGetter;
            }

            IsForeignKey(prop =>
            {
                var attributes = prop.Attributes<TAttribute>();

                return attributes != null && attributes.Select(isForeignKeyGetter).Any();
            });
        }

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <param name="isForeignKeyGetter">The is foreign key getter.</param>
        public void IsForeignKey(Func<PropertyInfo, bool> isForeignKeyGetter)
        {
            _configuration.IsForeignKeyGetters.Insert(0, isForeignKeyGetter);
        }

        /// <summary>
        /// Appends a parser to find a computed a property.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="computedGetter">The computed getter.</param>
        public void ComputedColumn<TAttribute>(Func<TAttribute, bool> computedGetter) where TAttribute : Attribute
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
        public void ComputedColumn(Func<PropertyInfo, bool> computedGetter)
        {
            _configuration.ComputedColumnGetters.Insert(0, computedGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property.
        /// </summary>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKey(Func<PropertyInfo, string> foreignKeyGetter)
        {
            _configuration.ColumnForeignKeyGetters.Insert(0, foreignKeyGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKey<TAttribute>(Func<TAttribute, string> foreignKeyGetter) where TAttribute : Attribute
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
        public void ForeignKeyLinks(Func<PropertyInfo, string> foreignKeyGetter)
        {
            _configuration.ColumnForeignKeyLinksGetters.Insert(0, foreignKeyGetter);
        }

        /// <summary>
        /// Appends a parser to find the association based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="foreignKeyGetter">The foreign key getter.</param>
        public void ForeignKeyLinks<TAttribute>(Func<TAttribute, string> foreignKeyGetter) where TAttribute : Attribute
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
        public void CollectionFilter(Func<PropertyInfo, string> filterGetter)
        {
            _configuration.CollectionFilterGetters.Insert(0, filterGetter);
        }

        /// <summary>
        /// Appends a parser to find the filter based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="filterGetter">The filter getter.</param>
        public void CollectionFilter<TAttribute>(Func<TAttribute, string> filterGetter) where TAttribute : Attribute
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
            return _configuration;
        }

    }
}