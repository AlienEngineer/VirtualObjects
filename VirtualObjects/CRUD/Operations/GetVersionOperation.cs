using System.Collections.Generic;

namespace VirtualObjects.CRUD.Operations
{
    class GetVersionOperation : Operation
    {
        public GetVersionOperation(string commandText, IEntityInfo entityInfo) 
            : base(commandText, entityInfo)
        {
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters, SessionContext sessionContext)
        {
            return connection.ExecuteScalar(commandText, parameters) as byte[];
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.KeyColumns;
        }
    }
}