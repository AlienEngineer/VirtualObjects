using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

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

            while (hasMore || reader.Read())
            {
                var entity = queryInfo.MakeEntity(sessionContext.Session);
                // var mapped = new MapResult {Entity = entity, HasMore = false}; 
                var mapped = queryInfo.MapEntity(entity, reader);
                var casted = queryInfo.EntityCast(mapped.Entity);

                yield return casted;

                hasMore = mapped.HasMore;
            }

            sessionContext.Connection.KeepAlive = keepAlive;
            sessionContext.Connection.Close();

        }
    }


}