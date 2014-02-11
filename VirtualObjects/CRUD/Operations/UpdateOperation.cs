using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CRUD.Operations
{
    class UpdateOperation : Operation
    {
        public UpdateOperation(string commandText, IEntityInfo entityInfo) 
            : base(commandText, entityInfo)
        {
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters, SessionContext sessionContext)
        {
            return connection.ExecuteNonQuery(commandText, parameters) > 0 ? entityModel : null;
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.Columns.Where(e => !e.IsVersionControl);
        }
    }
}