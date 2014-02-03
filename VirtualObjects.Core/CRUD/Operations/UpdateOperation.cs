using System.Collections.Generic;
using VirtualObjects.Config;

namespace VirtualObjects.Core.CRUD.Operations
{
    class UpdateOperation : Operation
    {
        public UpdateOperation(string commandText, IEntityInfo entityInfo) 
            : base(commandText, entityInfo)
        {
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            connection.ExecuteNonQuery(commandText, parameters);

            return entityModel;
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.Columns;
        }
    }
}