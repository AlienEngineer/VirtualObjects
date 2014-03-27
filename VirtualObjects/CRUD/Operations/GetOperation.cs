using System.Collections.Generic;
using System.Data;
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

            _context = new MapperContext
            {
                EntityInfo = entityInfo,
                OutputType = entityInfo.EntityType
            };
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters, SessionContext sessionContext)
        {
            var keepAlive = connection.KeepAlive;
            connection.KeepAlive = true;
            IDataReader reader = null;

            try
            {
                reader = connection.ExecuteReader(commandText, parameters);

                if ( !reader.Read() )
                {
                    reader.Close();
                    return null;
                }

                _entityProvider.PrepareProvider(entityInfo.EntityType, sessionContext);

                _mapper.PrepareMapper(_context);
                var proxy = _entityProvider.CreateEntity(entityInfo.EntityType);

                return _mapper.MapEntity(reader, proxy, _context);
            }
            finally
            {
                if (reader != null) reader.Close();

                //
                // Restore keep alive flag;
                connection.KeepAlive = keepAlive;
                connection.Close();
            }
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.KeyColumns;
        }
    }
}