using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VirtualObjects.Config;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Queries.Mapping
{
    class CollectionEntitiesMapper : IEntitiesMapper
    {
        private readonly IEntityBag _entityBag;
        private readonly IEntityProvider _entityProvider;
        private readonly IEnumerable<IEntityMapper> _entityMappers;

        public CollectionEntitiesMapper(IEntityBag entityBag, IEntityProvider entityProvider, IEnumerable<IEntityMapper> entityMappers)
        {
            _entityBag = entityBag;
            _entityProvider = entityProvider;
            _entityMappers = entityMappers;
        }

        public IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader, IQueryInfo queryInfo, SessionContext sessionContext)
        {
            // reader = new BlockingDataReader(reader);

            return MapEntities(reader, queryInfo, typeof(TEntity), sessionContext).Cast<TEntity>();
        }

        public IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType, SessionContext sessionContext)
        {
            var result = new List<Object>();
            try
            {
                var entityInfo = queryInfo.EntityInfo;// ?? _entityBag[outputType];

                var context = new MapperContext
                {
                    EntityInfo = entityInfo,
                    OutputType = outputType,
                    EntityProvider = _entityProvider,
                    EntityBag = _entityBag,
                    QueryInfo = queryInfo
                };

                if (entityInfo != null && entityInfo.EntityProvider != null)
                {
                    context.EntityProvider = entityInfo.EntityProvider;
                }

                var entityMapper =
                    (entityInfo != null && entityInfo.EntityMapper != null) ?
                        entityInfo.EntityMapper :
                        _entityMappers.FirstOrDefault(e => e.CanMapEntity(context));


                if (entityMapper == null)
                {
                    throw new MappingException(Errors.Mapping_OutputTypeNotSupported, context);
                }



                //
                // This line enables about 50% more code eficiency.
                //
                _entityProvider.PrepareProvider(context.OutputType, sessionContext);
                entityMapper.PrepareMapper(context);


                while (context.Read || reader.Read())
                {
                    result.Add(entityMapper.MapEntity(reader, context.CreateEntity(), context));
                }

                return result;
            }
            catch (Exception ex)
            {
                if (ex is MappingException)
                {
                    throw;
                }

                throw new MappingException("Unable to map the query into [{Name}]", outputType, ex);

            }
            finally
            {
                reader.Close();
            }
        }
    }
}
