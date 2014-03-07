using System.Collections.Generic;

namespace VirtualObjects.CRUD.Operations
{
    class CountOperation : Operation
    {
        readonly static IEnumerable<IEntityColumnInfo> _stub = new List<IEntityColumnInfo>();
        public CountOperation(string commandText, IEntityInfo entityInfo)
            : base(commandText, entityInfo)
        {
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters, SessionContext sessionContext)
        {
            return connection.ExecuteScalar(commandText, parameters);
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return _stub;
        }
    }
}
