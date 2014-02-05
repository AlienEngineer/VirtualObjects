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

        public IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader, IQueryInfo queryInfo)
        {
            // reader = new BlockingDataReader(reader);

            return MapEntities(reader, queryInfo, typeof (TEntity)).Cast<TEntity>();
        }

        public IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType)
        {
            var result = new List<Object>();
            var context = new MapperContext
            {
                EntityInfo = _mapper.Map(outputType),
                OutputType = outputType,
                EntityProvider = _entityProvider,
                Mapper = _mapper,
                QueryInfo = queryInfo
            };

            var entityMapper = _entityMappers.FirstOrDefault(e => e.CanMapEntity(context));

            if ( entityMapper == null )
            {
                throw new MappingException(Errors.Mapping_OutputTypeNotSupported, context);
            }

            //
            // This line enables about 50% more code eficiency.
            //
            try
            {

                _entityProvider.PrepareProvider(context.OutputType);
                entityMapper.PrepareMapper(context);

                while (context.Read || reader.Read())
                {
                    result.Add(entityMapper.MapEntity(reader, context.CreateEntity(), context));
                }

                reader.Close();

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
        }
    }
}
