using System;

namespace VirtualObjects
{
    public interface IOperation
    {
        String CommandText { get; }
        object Execute(SessionContext sessionContext);
        IOperation PrepareOperation(object entityModel);
    }
}