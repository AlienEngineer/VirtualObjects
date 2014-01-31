
namespace VirtualObjects.Core.CRUD
{
    public interface IOperations
    {
        IOperation InsertOperation { get; }
        IOperation UpdateOperation { get; }
        IOperation DeleteOperation { get; }
        IOperation GetOperation { get; }
    }
}