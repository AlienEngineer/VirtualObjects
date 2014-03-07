using System;

namespace VirtualObjects.CRUD
{
    class OperationsExecutor : IOperationsExecutor
    {
        private readonly IOperations _entityOperations;

        public OperationsExecutor(IOperations entityOperations)
        {
            _entityOperations = entityOperations;
        }

        public Object Insert(Object entityModel, SessionContext sessionContext)
        {
            return _entityOperations.InsertOperation
                .PrepareOperation(entityModel)
                .Execute(sessionContext);
        }

        public Object Update(Object entityModel, SessionContext sessionContext)
        {
            return _entityOperations.UpdateOperation
                .PrepareOperation(entityModel)
                .Execute(sessionContext);
        }

        public Object Delete(Object entityModel, SessionContext sessionContext)
        {
            return _entityOperations.DeleteOperation
                .PrepareOperation(entityModel)
                .Execute(sessionContext);
        }

        public Object Get(Object entityModel, SessionContext sessionContext)
        {
            return _entityOperations.GetOperation
                .PrepareOperation(entityModel)
                .Execute(sessionContext);
        }

        public byte[] GetVersion(object entityModel, SessionContext sessionContext)
        {
            return _entityOperations.GetVersionOperation
                .PrepareOperation(entityModel)
                .Execute(sessionContext) as byte[];
        }

        public int Count(SessionContext sessionContext)
        {
            return (int)_entityOperations.CountOperation.Execute(sessionContext);
        }
    }

    class OperationsExecutor<T> : OperationsExecutor, IOperationsExecutor<T>
    {
        public OperationsExecutor( IOperations entityOperations) 
            : base(entityOperations)
        {
        }

        public T Insert(T entityModel, SessionContext sessionContext)
        {
            return (T)base.Insert(entityModel, sessionContext);
        }

        public T Update(T entityModel, SessionContext sessionContext)
        {
            return (T)base.Update(entityModel, sessionContext);
        }

        public T Delete(T entityModel, SessionContext sessionContext)
        {
            return (T)base.Delete(entityModel, sessionContext);
        }

        public T Get(T entityModel, SessionContext sessionContext)
        {
            return (T)base.Get(entityModel, sessionContext);
        }
    }
}