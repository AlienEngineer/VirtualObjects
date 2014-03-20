using System.Collections.Generic;

namespace VirtualObjects.CRUD.Operations
{
    class CountOperation : Operation
    {
        readonly static IEnumerable<IEntityColumnInfo> _stub = new List<IEntityColumnInfo>();

        IOperation countOp;

        public CountOperation(string commandText, IEntityInfo entityInfo)
            : base(commandText, entityInfo)
        {
            countOp = this;
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters, SessionContext sessionContext)
        {
            countOp = new ScalarOperationExecutor(connection.CreateCommand(commandText), parameters);
            return countOp.Execute(sessionContext);
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return _stub;
        }
    }

}
