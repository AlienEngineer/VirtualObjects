using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using VirtualObjects.Exceptions;

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
            var result = new List<object>();
            var hasMore = false;
            try
            {
                while ( hasMore || reader.Read() )
                {
                    var entity = queryInfo.MakeEntity(sessionContext.Session);
                    var mapped = queryInfo.MapEntity(entity, reader);
                    var casted = queryInfo.EntityCast(mapped.Entity);

                    result.Add(casted);

                    hasMore = mapped.HasMore;
                }

                return result;
            }
            catch (Exception ex)
            {
                if (ex is MappingException)
                {
                    throw;
                }

                throw new MappingException(Errors.EntitiesMapper_UnableToMapType, outputType, ex);

            }
            finally
            {
                reader.Close();
            }
        }
    }
}