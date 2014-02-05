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

        public Object Insert(Object entityModel, IConnection connection)
        {
            return _entityOperations.InsertOperation
                .PrepareOperation(entityModel)
                .Execute(connection);
        }

        public Object Update(Object entityModel, IConnection connection)
        {
            return _entityOperations.UpdateOperation
                .PrepareOperation(entityModel)
                .Execute(connection);
        }

        public Object Delete(Object entityModel, IConnection connection)
        {
            return _entityOperations.DeleteOperation
                .PrepareOperation(entityModel)
                .Execute(connection);
        }

        public Object Get(Object entityModel, IConnection connection)
        {
            return _entityOperations.GetOperation
                .PrepareOperation(entityModel)
                .Execute(connection);
        }

    }

    class OperationsExecutor<T> : OperationsExecutor, IOperationsExecutor<T>
    {
        public OperationsExecutor( IOperations entityOperations) 
            : base(entityOperations)
        {
        }

        public T Insert(T entityModel, IConnection connection)
        {
            return (T)base.Insert(entityModel, connection);
        }

        public T Update(T entityModel, IConnection connection)
        {
            return (T)base.Update(entityModel, connection);
        }

        public T Delete(T entityModel, IConnection connection)
        {
            return (T)base.Delete(entityModel, connection);
        }

        public T Get(T entityModel, IConnection connection)
        {
            return (T)base.Get(entityModel, connection);
        }
    }
}