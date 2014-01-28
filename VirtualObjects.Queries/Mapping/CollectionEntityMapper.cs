using System.Collections.Generic;
using System.Data;
using System.Linq;
using VirtualObjects.Config;

namespace VirtualObjects.Queries.Mapping
{
    class CollectionEntityMapper : IEntitiesMapper
    {
        private readonly IMapper _mapper;
        private readonly IEntityProvider _entityProvider;
        private readonly IEnumerable<IEntityMapper> _entityMappers; 

        public CollectionEntityMapper(IMapper mapper, IEntityProvider entityProvider, IEnumerable<IEntityMapper> entityMappers)
        {
            _mapper = mapper;
            _entityProvider = entityProvider;
            _entityMappers = entityMappers;
        }

        public IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader)
        {
            var result = new List<TEntity>();
            var context = new MapperContext
            {
                EntityInfo = _mapper.Map(typeof(TEntity)),
                OutputType = typeof(TEntity),
                EntityProvider = _entityProvider
            };

            var entityMapper = _entityMappers.FirstOrDefault(e => e.CanMapEntity(context));

            if (entityMapper == null)
            {
                throw new MappingException(Errors.Mapping_OutputTypeNotSupported, context);
            }

            _entityProvider.PrepareProvider(context.OutputType);
            entityMapper.PrepareMapper(context);

            while(reader.Read())
            {
                result.Add((TEntity)entityMapper.MapEntity(reader, context.CreateEntity(), context));
            }

            return result;
        }
    }
}
