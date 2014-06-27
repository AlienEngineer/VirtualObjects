using System;
using System.Collections.Generic;
using System.Reflection;

namespace VirtualObjects.Config
{
    /// <summary>
    /// Translates the configurations into collections of metadata.
    /// 
    /// This metadata reflects key information about the entity models. POCO classes.
    /// </summary>
    public interface IConfigurationTranslator
    {
        /// <summary>
        /// Gets the column ignore getters.
        /// </summary>
        /// <value>
        /// The column ignore getters.
        /// </value>
        IList<Func<PropertyInfo, Boolean>> ColumnIgnoreGetters { get; }
        /// <summary>
        /// Gets the column name getters.
        /// </summary>
        /// <value>
        /// The column name getters.
        /// </value>
        IList<Func<PropertyInfo, String>> ColumnNameGetters { get; }
        /// <summary>
        /// Gets the column key getters.
        /// </summary>
        /// <value>
        /// The column key getters.
        /// </value>
        IList<Func<PropertyInfo, Boolean>> ColumnKeyGetters { get; }
        /// <summary>
        /// Gets the column identity getters.
        /// </summary>
        /// <value>
        /// The column identity getters.
        /// </value>
        IList<Func<PropertyInfo, Boolean>> ColumnIdentityGetters { get; }
        /// <summary>
        /// Gets the entity name getters.
        /// </summary>
        /// <value>
        /// The entity name getters.
        /// </value>
        IList<Func<Type, String>> EntityNameGetters { get; }
        /// <summary>
        /// Gets the entity schema getters.
        /// </summary>
        /// <value>
        /// The entity schema getters.
        /// </value>
        IList<Func<Type, String>> EntitySchemaGetters { get; }
        /// <summary>
        /// Gets the column foreign key getters.
        /// </summary>
        /// <value>
        /// The column foreign key getters.
        /// </value>
        IList<Func<PropertyInfo, String>> ColumnForeignKeyGetters { get; }
        /// <summary>
        /// Gets the column foreign key links getters.
        /// </summary>
        /// <value>
        /// The column foreign key links getters.
        /// </value>
        IList<Func<PropertyInfo, String>> ColumnForeignKeyLinksGetters { get; }
        /// <summary>
        /// Gets the column version field getters.
        /// </summary>
        /// <value>
        /// The column version field getters.
        /// </value>
        IList<Func<PropertyInfo, Boolean>> ColumnVersionFieldGetters { get; }
        /// <summary>
        /// Gets the computed column getters.
        /// </summary>
        /// <value>
        /// The computed column getters.
        /// </value>
        IList<Func<PropertyInfo, Boolean>> ComputedColumnGetters { get; }

        /// <summary>
        /// Gets the collection filter getters.
        /// </summary>
        /// <value>
        /// The collection filter getters.
        /// </value>
        IList<Func<PropertyInfo, String>> CollectionFilterGetters { get; }
    }
}