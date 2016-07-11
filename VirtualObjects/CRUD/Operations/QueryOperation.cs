using System.Data;
using System.Linq;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.CRUD.Operations
{
    internal class QueryOperation : IOperation
    {
        private readonly IEntityInfo _entityInfo;
        private QueryCommand _queryCommand;
        private readonly IEntitiesMapper _mapper;
        private readonly MapperContext _context;

        public QueryOperation(IEntityInfo entityInfo) : this(entityInfo, new EntityModelEntitiesMapper())
        {
            
        }

        public QueryOperation(IEntityInfo entityInfo, IEntitiesMapper mapper)
        {
            _entityInfo = entityInfo;
            _mapper = mapper;

            _context = new MapperContext
            {
                EntityInfo = entityInfo,
                OutputType = entityInfo.EntityType
            };
        }

        public string CommandText { get; set; }

        public object Execute(SessionContext sessionContext)
        {
            var connection = sessionContext.Connection;
            var commandText = _queryCommand.Command;
            var parameters = _queryCommand.Parameters.ToDictionary(
                e => e.Name,
                e => (IOperationParameter)new OperationParameter
                {
                    Name = e.Name,
                    Value = e.Value
                });

            var reader = connection.ExecuteReader(commandText, parameters);

            return _mapper.MapEntities(reader, new QueryInfo
            {
                MapEntity = _entityInfo.MapEntity,
                EntityCast = _entityInfo.EntityCast,
                MakeEntity = _entityInfo.EntityProxyFactory
            }, _context.OutputType, sessionContext);
        }

        public IOperation PrepareOperation(object entityModel)
        {
            var queryCommand = entityModel as QueryCommand;

            if (queryCommand == null)
            {
                throw new ExecutionException(Errors.Operations_Invalid_Argument);
            }

            _queryCommand = queryCommand;

            return this;
        }
    }
}