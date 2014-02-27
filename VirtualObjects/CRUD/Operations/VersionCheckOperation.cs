using VirtualObjects.Exceptions;

namespace VirtualObjects.CRUD.Operations
{
    class VersionCheckOperation : IOperation
    {
        private readonly IOperation _operation;
        private readonly IEntityInfo _entityInfo;
        private object _entityModel;

        public VersionCheckOperation(IOperation operation, IEntityInfo entityInfo)
        {
            _operation = operation;
            _entityInfo = entityInfo;
        }

        public string CommandText { get { return _operation.CommandText; } }

        public object Execute(SessionContext sessionContext)
        {
            if (_entityInfo.VersionControl != null)
            {
                var currentSersion = _entityInfo.VersionControl.GetFieldFinalValue(_entityModel) as byte[];

                if (currentSersion == null)
                {
                    throw new ExecutionException(Errors.Operations_VersionControl_NotSupplied);
                }
                var dataSourceVersion = _entityInfo.Operations.GetVersionOperation.PrepareOperation(_entityModel).Execute(sessionContext) as byte[];

                if (dataSourceVersion != null && dataSourceVersion.GetHashCode() > currentSersion.GetHashCode())
                {
                    throw new ExecutionException(Errors.Operations_VersionControlError);
                }
            }

            return _operation.Execute(sessionContext);
        }

        public IOperation PrepareOperation(object entityModel)
        {
            _entityModel = entityModel;
            _operation.PrepareOperation(_entityModel);
            return this;
        }
    }
}