using System;

namespace VirtualObjects.Core.CRUD
{
    public interface IOperation
    {
        Object Execute(IConnection connection);
        IOperation PrepareOperation(object entityModel);
    }
}