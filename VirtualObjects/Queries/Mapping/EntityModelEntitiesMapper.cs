using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VirtualObjects.Config;
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
            var result = new List<Object>();
            try
            {
                var mapperContext = new MapperContext { EntityInfo = queryInfo.EntityInfo };
                while (reader.Read())
                {
                    result.Add(
                            queryInfo.EntityInfo.EntityMapper.MapEntity(
                                reader, 
                                queryInfo.EntityInfo.EntityProxyFactory(sessionContext.Session),
                                mapperContext
                            )
                    );
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