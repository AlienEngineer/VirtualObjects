
using System.Collections.Generic;

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
        /// Gets the count operation.
        /// </summary>
        /// <value>
        /// The count operation.
        /// </value>
        IOperation CountOperation { get; }

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

        /// <summary>
        /// Gets the get version operation.
        /// </summary>
        /// <value>
        /// The get version operation.
        /// </value>
        IOperation GetVersionOperation { get; }

        /// <summary>
        /// Gets or sets the query operation.
        /// </summary>
        /// <value>
        /// The query operation.
        /// </value>
        IOperation QueryOperation { get; set; }
    }
}