using System;
using System.Collections.Generic;
using System.Data.Common;
using VirtualObjects.Config;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryInfo
    {
        Func<object, object[], Object> MapEntity { get; set; }
        /// <summary>
        /// Gets or sets the entity information.
        /// </summary>
        /// <value>
        /// The entity information.
        /// </value>
        IEntityInfo EntityInfo { get; set; }
        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        string CommandText { get; }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        DbCommand Command { get; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        IDictionary<string, IOperationParameter> Parameters { get; set; }

        /// <summary>
        /// Gets the predicated columns.
        /// </summary>
        /// <value>
        /// The predicated columns.
        /// </value>
        IList<IEntityColumnInfo> PredicatedColumns { get; }

        /// <summary>
        /// Gets the type of the output.
        /// </summary>
        /// <value>
        /// The type of the output.
        /// </value>
        Type OutputType { get; }

        /// <summary>
        /// Gets or sets the entity mapper.
        /// </summary>
        /// <value>
        /// The entity mapper.
        /// </value>
        IEntitiesMapper EntitiesMapper { get; set; }
    }
}