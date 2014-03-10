using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.CRUD.Operations
{
    class ScalarOperationExecutor : IOperation
    {

        private readonly IDbCommand command;
        private readonly IDictionary<string, IOperationParameter> parameters;

        public ScalarOperationExecutor(IDbCommand command, IDictionary<string, IOperationParameter> parameters)
        {
            this.parameters = parameters;
            this.command = command;
        }

        #region IOperation Members

        public string CommandText
        {
            get { throw new System.NotImplementedException(); }
        }

        public object Execute(SessionContext sessionContext)
        {
            return sessionContext.Connection.ExecuteScalar(command, parameters);
        }

        public IOperation PrepareOperation(object entityModel)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
