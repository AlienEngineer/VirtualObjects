namespace VirtualObjects
{
    public interface IOperationsProvider
    {
        IOperations CreateOperations(IEntityInfo entityInfo);
    }
}