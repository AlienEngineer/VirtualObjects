namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOperationsProvider
    {
        /// <summary>
        /// Creates the operations.
        /// </summary>
        /// <param name="entityInfo">The entity information.</param>
        /// <returns></returns>
        IOperations CreateOperations(IEntityInfo entityInfo);
    }
}