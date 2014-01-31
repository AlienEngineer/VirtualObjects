using VirtualObjects.Config;

namespace VirtualObjects.Core.CRUD.Operations
{
    class DeleteOperation: UpdateOperation
    {
        public DeleteOperation(string commandText, IEntityInfo entityInfo) 
            : base(commandText, entityInfo)
        {
        }
    }
}