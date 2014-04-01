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

        public IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType, SessionContext context)
        {
            var hasMore = false;
            var keepAlive = context.Connection.KeepAlive;
            context.Connection.KeepAlive = true;

            while ( hasMore || reader.Read() )
            {
                var entity = queryInfo.MakeEntity(context.Session);
                var mapped = queryInfo.MapEntity(entity, reader);
                var casted = queryInfo.EntityCast(mapped.Entity);

                yield return casted;

                hasMore = mapped.HasMore;
            }

            context.Connection.KeepAlive = keepAlive;
            context.Connection.Close();
        }
    }
}