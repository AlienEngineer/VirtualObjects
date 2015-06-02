using System;
using System.Collections.Generic;
using System.Reflection;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntityColumnInfo
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        int Index { get; set; }
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        string ColumnName { get; }

        /// <summary>
        /// Gets or sets the foreign key links.
        /// </summary>
        /// <value>
        /// The foreign key links.
        /// </value>
        List<KeyValuePair<IEntityColumnInfo, IEntityColumnInfo>> ForeignKeyLinks { get; set; }

        /// <summary>
        /// Gets a value indicating whether [is key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is key]; otherwise, <c>false</c>.
        /// </value>
        bool IsKey { get; }

        /// <summary>
        /// Gets a value indicating whether [is identity].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is identity]; otherwise, <c>false</c>.
        /// </value>
        bool IsIdentity { get; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        PropertyInfo Property { get; }

        /// <summary>
        /// Gets the entity information.
        /// </summary>
        /// <value>
        /// The entity information.
        /// </value>
        IEntityInfo EntityInfo { get;  }

        /// <summary>
        /// Gets or sets the foreign key.
        /// </summary>
        /// <value>
        /// The foreign key.
        /// </value>
        IEntityColumnInfo ForeignKey { get; set; }

        /// <summary>
        /// Gets the bind name or the name.
        /// </summary>
        /// <value>
        /// The name of the bind or.
        /// </value>
        string BindOrName { get; }

        /// <summary>
        /// Gets the value getter.
        /// </summary>
        /// <value>
        /// The value getter.
        /// </value>
        Func<object, object> ValueGetter { get; }

        /// <summary>
        /// Gets the value setter.
        /// </summary>
        /// <value>
        /// The value setter.
        /// </value>
        Action<object, object> ValueSetter { get; }

        /// <summary>
        /// Gets a value indicating whether [is version control].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is version control]; otherwise, <c>false</c>.
        /// </value>
        bool IsVersionControl { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [is computed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is computed]; otherwise, <c>false</c>.
        /// </value>
        bool IsComputed { get; set; }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        object DefaultValue { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [inject nulls].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [inject nulls]; otherwise, <c>false</c>.
        /// </value>
        bool InjectNulls { get; set; }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="value">The value.</param>
        object SetValue(object entity, object value);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        object GetValue(object entity);

        /// <summary>
        /// Gets the field final value.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        object GetFieldFinalValue(object entity);

        /// <summary>
        /// Sets the field final value.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="value">The value.</param>
        object SetFieldFinalValue(object entity, object value);

        /// <summary>
        /// Gets the last bind.
        /// </summary>
        /// <returns></returns>
        IEntityColumnInfo GetLastBind();
        
        
    }
}