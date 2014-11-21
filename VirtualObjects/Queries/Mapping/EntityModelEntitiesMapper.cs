using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace VirtualObjects.Queries.Mapping
{
    class EntityModelEntitiesMapper : IEntitiesMapper
    {

        public IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader, IQueryInfo queryInfo, SessionContext sessionContext)
        {
            return MapEntities(reader, queryInfo, typeof(TEntity), sessionContext).Cast<TEntity>();
        }

        public IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType, SessionContext sessionContext)
        {

            var hasMore = false;
            var keepAlive = sessionContext.Connection.KeepAlive;
            sessionContext.Connection.KeepAlive = true;
            
            //
            // Data buffer. Avoiding the creation of memory objects.
            var data = new object[queryInfo.EntityInfo.Columns.Count];

            while (hasMore || reader.Read())
            {
                var entity = queryInfo.MakeEntity(sessionContext.Session);
                // var mapped = new MapResult {Entity = entity, HasMore = false}; 
                var mapped = queryInfo.MapEntity(entity, reader, data);
                var casted = queryInfo.EntityCast(mapped.Entity);

                yield return casted;

                //
                // HasMore is a flag set by the MapEntity method.
                // This is used when mapping collection properties.
                hasMore = mapped.HasMore;
            }

            sessionContext.Connection.KeepAlive = keepAlive;
            sessionContext.Connection.Close();

        }
    }


}