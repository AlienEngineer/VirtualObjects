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
        private readonly IMapper _mapper;
        private readonly IEntityProvider _entityProvider;
        private readonly IEnumerable<IEntityMapper> _entityMappers;

        public CollectionEntitiesMapper(IMapper mapper, IEntityProvider entityProvider, IEnumerable<IEntityMapper> entityMappers)
        {
            _mapper = mapper;
            _entityProvider = entityProvider;
            _entityMappers = entityMappers;
        }

        public IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader, IQueryInfo queryInfo, SessionContext sessionContext)
        {
            // reader = new BlockingDataReader(reader);

            return MapEntities(reader, queryInfo, typeof (TEntity), sessionContext).Cast<TEntity>();
        }

        public IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType, SessionContext sessionContext)
        {
            var result = new List<Object>();

            var entityInfo = _mapper.Map(outputType);

            var context = new MapperContext
            {
                EntityInfo = entityInfo,
                OutputType = outputType,
                EntityProvider = _entityProvider,
                Mapper = _mapper,
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

            if ( entityMapper == null )
            {
                throw new MappingException(Errors.Mapping_OutputTypeNotSupported, context);
            }

            try
            {

                //
                // This line enables about 50% more code eficiency.
                //
                _entityProvider.PrepareProvider(context.OutputType, sessionContext);
                entityMapper.PrepareMapper(context);

                while ( context.Read || reader.Read() )
                {
                    result.Add(entityMapper.MapEntity(reader, context.CreateEntity(), context));
                }

                return result;
            }
            catch ( Exception ex )
            {
                if ( ex is MappingException )
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
