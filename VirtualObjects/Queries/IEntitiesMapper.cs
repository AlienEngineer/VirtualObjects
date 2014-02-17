using System;
using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntitiesMapper
    {
        /// <summary>
        /// Maps the entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="context">The context.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader, IQueryInfo context, SessionContext sessionContext);

        /// <summary>
        /// Maps the entities.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="queryInfo">The query information.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType, SessionContext sessionContext);
    }
}