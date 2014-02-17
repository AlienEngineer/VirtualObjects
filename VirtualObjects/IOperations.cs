
namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOperations
    {
        /// <summary>
        /// Gets the insert operation.
        /// </summary>
        /// <value>
        /// The insert operation.
        /// </value>
        IOperation InsertOperation { get; }

        /// <summary>
        /// Gets the update operation.
        /// </summary>
        /// <value>
        /// The update operation.
        /// </value>
        IOperation UpdateOperation { get; }

        /// <summary>
        /// Gets the delete operation.
        /// </summary>
        /// <value>
        /// The delete operation.
        /// </value>
        IOperation DeleteOperation { get; }

        /// <summary>
        /// Gets the get operation.
        /// </summary>
        /// <value>
        /// The get operation.
        /// </value>
        IOperation GetOperation { get; }
    }
}