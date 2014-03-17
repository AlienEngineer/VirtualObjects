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
            return MapEntities(reader, queryInfo, typeof(TEntity), sessionContext).Select(e => (TEntity)e);
        }

        public IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType, SessionContext sessionContext)
        {
            var result = new List<dynamic>();
            try
            {
                while (reader.Read())
                {
                    result.Add(
                        queryInfo.MapEntity(
                            queryInfo.MakeEntity(sessionContext.Session),
                            reader.GetValues()
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