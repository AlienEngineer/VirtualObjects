using System;

namespace VirtualObjects
{
    public interface IOperation
    {
        String CommandText { get; }
        Object Execute(IConnection connection);
        IOperation PrepareOperation(object entityModel);
    }
}