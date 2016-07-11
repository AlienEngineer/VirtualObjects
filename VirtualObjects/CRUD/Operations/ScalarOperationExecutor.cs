using System;
using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.CRUD.Operations
{
    class ScalarOperationExecutor : IOperation
    {

        private readonly IDbCommand _command;
        private readonly IDictionary<string, IOperationParameter> _parameters;

        public ScalarOperationExecutor(IDbCommand command, IDictionary<string, IOperationParameter> parameters)
        {
            _parameters = parameters;
            _command = command;
        }

        #region IOperation Members

        public string CommandText
        {
            get { throw new NotImplementedException(); }
        }

        public object Execute(SessionContext sessionContext)
        {
            return sessionContext.Connection.ExecuteScalar(_command, _parameters);
        }

        public IOperation PrepareOperation(object entityModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
