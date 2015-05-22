using System;
using System.Collections.Generic;
using System.Data;
using VirtualObjects.Config;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntityInfo
    {
        
        /// <summary>
        /// Gets or sets the key hash code.
        /// </summary>
        /// <value>
        /// The key hash code.
        /// </value>
        Func<Object, int> KeyHashCode { get; set; }
        /// <summary>
        /// Gets or sets the entity factory.
        /// </summary>
        /// <value>
        /// The entity factory.
        /// </value>
        Func<object> EntityFactory { get; set; }

        /// <summary>
        /// Gets or sets the entity proxy factory.
        /// </summary>
        /// <value>
        /// The entity proxy factory.
        /// </value>
        Func<ISession, object> EntityProxyFactory { get; set; }

        /// <summary>
        /// Gets or sets the map entity.
        /// </summary>
        /// <value>
        /// The map entity.
        /// </value>
        Func<Object, IDataReader, MapResult> MapEntity { get; set; }

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
        String EntityName { get; set; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        IList<IEntityColumnInfo> Columns { get; set; }

        /// <summary>
        /// Gets the key columns.
        /// </summary>
        /// <value>
        /// The key columns.
        /// </value>
        IList<IEntityColumnInfo> KeyColumns { get; set; }

        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        IEntityColumnInfo Identity { get; set; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        Type EntityType { get; set; }

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
        IOperations Operations { get; set; }
        /// <summary>
        /// Gets the foreign keys.
        /// </summary>
        /// <value>
        /// The foreign keys.
        /// </value>
        IList<IEntityColumnInfo> ForeignKeys { get; set; }
        /// <summary>
        /// Gets the entity provider.
        /// </summary>
        /// <value>
        /// The entity provider.
        /// </value>
        IEntityProvider EntityProvider { get; set; }
        /// <summary>
        /// Gets the entity mapper.
        /// </summary>
        /// <value>
        /// The entity mapper.
        /// </value>
        IEntityMapper EntityMapper { get; set; }

        /// <summary>
        /// Gets the version control column.
        /// </summary>
        /// <value>
        /// The version control.
        /// </value>
        IEntityColumnInfo VersionControl { get; set; }

        /// <summary>
        /// Entities the cast.
        /// </summary>
        /// <value>
        /// The entity cast.
        /// </value>
        ///   <returns></returns>
        Func<Object, Object> EntityCast { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Mapping State { get; set; }
    }
}