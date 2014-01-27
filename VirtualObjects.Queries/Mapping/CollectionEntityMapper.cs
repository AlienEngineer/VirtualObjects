using System.Collections.Generic;
using System.Data;
using VirtualObjects.Config;

namespace VirtualObjects.Queries.Mapping
{
    class CollectionEntityMapper : IEntitiesMapper
    {
        private readonly IMapper _mapper;
        private readonly IEntityProvider _entityProvider;
        readonly IEntityMapper orderedMapper = new OrderedEntityMapper();

        public CollectionEntityMapper(IMapper mapper, IEntityProvider entityProvider)
        {
            _mapper = mapper;
            _entityProvider = entityProvider;
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


            while(reader.Read())
            {
                result.Add((TEntity)orderedMapper.MapEntity(reader, context.CreateEntity(), context));
            }

            return result;
        }
    }
}
