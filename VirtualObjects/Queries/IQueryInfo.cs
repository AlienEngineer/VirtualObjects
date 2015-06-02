using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryInfo
    {
        /// <summary>
        /// Gets or sets the sources.
        /// </summary>
        /// <value>
        /// The sources.
        /// </value>
        IList<IEntityInfo> Sources { get; set; }
        /// <summary>
        /// Gets or sets the on clauses.
        /// </summary>
        /// <value>
        /// The on clauses.
        /// </value>
        IList<OnClause> OnClauses { get; set; }
        /// <summary>
        /// Gets or sets the entity cast.
        /// </summary>
        /// <value>
        /// The entity cast.
        /// </value>
        Func<object, object> EntityCast { get; set; }
        /// <summary>
        /// Gets or sets the map entity.
        /// </summary>
        /// <value>
        /// The map entity.
        /// </value>
        Func<object, IDataReader, MapResult> MapEntity { get; set; }

        /// <summary>
        /// Gets or sets the make entity.
        /// </summary>
        /// <value>
        /// The make entity.
        /// </value>
        Func<ISession, object> MakeEntity { get; set; }

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