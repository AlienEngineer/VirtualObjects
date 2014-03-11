using System;
using System.Collections.Generic;
using VirtualObjects.Queries;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntityInfo
    {

        Action<object, object[]> MapEntity { get; set; }
        /// <summary>
        /// Gets the <see cref="IEntityColumnInfo"/> with the specified property name.
        /// </summary>
        /// <value>
        /// The <see cref="IEntityColumnInfo"/>.
        /// </value>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        IEntityColumnInfo this[String propertyName] { get; }

        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        String EntityName { get; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        IList<IEntityColumnInfo> Columns { get; }

        /// <summary>
        /// Gets the key columns.
        /// </summary>
        /// <value>
        /// The key columns.
        /// </value>
        IList<IEntityColumnInfo> KeyColumns { get; }

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        IEntityColumnInfo Identity { get; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        Type EntityType { get; }

        /// <summary>
        /// Gets the field associated with.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IEntityColumnInfo GetFieldAssociatedWith(string name);

        /// <summary>
        /// Gets the key hash code.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        int GetKeyHashCode(Object obj);

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        IOperations Operations { get; }
        /// <summary>
        /// Gets the foreign keys.
        /// </summary>
        /// <value>
        /// The foreign keys.
        /// </value>
        IList<IEntityColumnInfo> ForeignKeys { get; }
        /// <summary>
        /// Gets the entity provider.
        /// </summary>
        /// <value>
        /// The entity provider.
        /// </value>
        IEntityProvider EntityProvider { get; }
        /// <summary>
        /// Gets the entity mapper.
        /// </summary>
        /// <value>
        /// The entity mapper.
        /// </value>
        IEntityMapper EntityMapper { get; }

        /// <summary>
        /// Gets the version control column.
        /// </summary>
        /// <value>
        /// The version control.
        /// </value>
        IEntityColumnInfo VersionControl { get; }
    }
}