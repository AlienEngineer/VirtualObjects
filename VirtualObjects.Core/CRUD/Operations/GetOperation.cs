using System.Collections.Generic;
using VirtualObjects.Config;
using VirtualObjects.Queries;

namespace VirtualObjects.Core.CRUD.Operations
{
    class GetOperation : Operation
    {
        private readonly IEntityMapper _mapper;

        public GetOperation(string commandText, IEntityInfo entityInfo, IEntityMapper mapper) 
            : base(commandText, entityInfo)
        {
            _mapper = mapper;
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, object> parameters)
        {
            var reader = connection.ExecuteReader(commandText, parameters);

            var context = new MapperContext
            {
                EntityInfo = entityInfo,
                OutputType = entityInfo.EntityType
            };

            _mapper.PrepareMapper(context);

            return _mapper.MapEntity(reader, entityModel, context);
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.KeyColumns;
        }
    }
}