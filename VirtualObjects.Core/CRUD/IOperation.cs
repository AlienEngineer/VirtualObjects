using System;

namespace VirtualObjects.Core.CRUD
{
    public interface IOperation
    {
        String CommandText { get; }
        Object Execute(IConnection connection);
        IOperation PrepareOperation(object entityModel);
    }
}