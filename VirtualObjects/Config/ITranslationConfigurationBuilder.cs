using System;
using System.Reflection;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITranslationConfigurationBuilder
    {
        /// <summary>
        /// Builds the mapper configuration.
        /// </summary>
        /// <returns></returns>
        ITranslationConfiguration Build();

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

        /// <summary>
        /// Appends a parser to find a computed a property.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="computedGetter">The computed getter.</param>
        void ComputedColumn<TAttribute>(Func<TAttribute, Boolean> computedGetter = null) where TAttribute : Attribute;

        /// <summary>
        /// Appends a parser to find a computed a property.
        /// </summary>
        /// <param name="computedGetter">The computed getter.</param>
        void ComputedColumn(Func<PropertyInfo, Boolean> computedGetter);


        /// <summary>
        /// Appends a parser to find the filter based on the property.
        /// </summary>
        /// <param name="filterGetter">The filter getter.</param>
        void CollectionFilter(Func<PropertyInfo, String> filterGetter);

        /// <summary>
        /// Appends a parser to find the filter based on the property attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="filterGetter">The filter getter.</param>
        void CollectionFilter<TAttribute>(Func<TAttribute, String> filterGetter) where TAttribute : Attribute;

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="isForeignKeyGetter">The is foreign key getter.</param>
        void IsForeignKey<TAttribute>(Func<TAttribute, Boolean> isForeignKeyGetter = null) where TAttribute : Attribute;

        /// <summary>
        /// Appends a parser to ignore a property.
        /// </summary>
        /// <param name="isForeignKeyGetter">The is foreign key getter.</param>
        void IsForeignKey(Func<PropertyInfo, Boolean> isForeignKeyGetter);
    }
}
