using System.Collections.Generic;
using VirtualObjects.Queries;

namespace VirtualObjects.CRUD.Operations
{
    class GetOperation : Operation
    {
        private readonly IEntityMapper _mapper;
        private readonly IEntityProvider _entityProvider;
        readonly MapperContext _context;

        public GetOperation(string commandText, IEntityInfo entityInfo, IEntityMapper mapper, IEntityProvider entityProvider) 
            : base(commandText, entityInfo)
        {
            _mapper = mapper;
            _entityProvider = entityProvider;
            _entityProvider.PrepareProvider(entityInfo.EntityType);
            
            _context = new MapperContext
            {
                EntityInfo = entityInfo,
                OutputType = entityInfo.EntityType,
                EntityProvider = _entityProvider
            };

            _mapper.PrepareMapper(_context);
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            var reader = connection.ExecuteReader(commandText, parameters);

            if (!reader.Read())
            {
                return null;
            }

            try
            {
                var proxy = _entityProvider.CreateEntity(entityInfo.EntityType);

                return _mapper.MapEntity(reader, proxy, _context);
            }
            finally
            {
                reader.Close();
            }
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.KeyColumns;
        }
    }
}