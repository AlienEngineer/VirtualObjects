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
            try
            {
                while (reader.Read())
                {
                    var entity = queryInfo.MakeEntity(sessionContext.Session);
                    var mapped = queryInfo.MapEntity(entity, reader.GetValues());
                    var casted = queryInfo.EntityCast(mapped);

                    result.Add(casted);
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