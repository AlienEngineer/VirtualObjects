using System;
using System.Collections.Generic;
using System.Reflection;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITranslationConfiguration
    {
        /// <summary>
        /// Gets the column ignore getters.
        /// </summary>
        /// <value>
        /// The column ignore getters.
        /// </value>
        IList<Func<PropertyInfo, bool>> ColumnIgnoreGetters { get; }
        /// <summary>
        /// Gets the column name getters.
        /// </summary>
        /// <value>
        /// The column name getters.
        /// </value>
        IList<Func<PropertyInfo, string>> ColumnNameGetters { get; }
        /// <summary>
        /// Gets the column key getters.
        /// </summary>
        /// <value>
        /// The column key getters.
        /// </value>
        IList<Func<PropertyInfo, bool>> ColumnKeyGetters { get; }
        /// <summary>
        /// Gets the column identity getters.
        /// </summary>
        /// <value>
        /// The column identity getters.
        /// </value>
        IList<Func<PropertyInfo, bool>> ColumnIdentityGetters { get; }
        /// <summary>
        /// Gets the entity name getters.
        /// </summary>
        /// <value>
        /// The entity name getters.
        /// </value>
        IList<Func<Type, string>> EntityNameGetters { get; }
        /// <summary>
        /// Gets the column foreign key getters.
        /// </summary>
        /// <value>
        /// The column foreign key getters.
        /// </value>
        IList<Func<PropertyInfo, string>> ColumnForeignKeyGetters { get; }
        /// <summary>
        /// Gets the column foreign key links getters.
        /// </summary>
        /// <value>
        /// The column foreign key links getters.
        /// </value>
        IList<Func<PropertyInfo, string>> ColumnForeignKeyLinksGetters { get; }
        /// <summary>
        /// Gets the column version field getters.
        /// </summary>
        /// <value>
        /// The column version field getters.
        /// </value>
        IList<Func<PropertyInfo, bool>> ColumnVersionFieldGetters { get; }
        /// <summary>
        /// Gets the computed column getters.
        /// </summary>
        /// <value>
        /// The computed column getters.
        /// </value>
        IList<Func<PropertyInfo, bool>> ComputedColumnGetters { get; }

        /// <summary>
        /// Gets the collection filter getters.
        /// </summary>
        /// <value>
        /// The collection filter getters.
        /// </value>
        IList<Func<PropertyInfo, string>> CollectionFilterGetters { get; }

        /// <summary>
        /// Gets or sets the is foreign getters.
        /// </summary>
        /// <value>
        /// The is foreign getters.
        /// </value>
        IList<Func<PropertyInfo, bool>> IsForeignKeyGetters { get; }
    }
}
