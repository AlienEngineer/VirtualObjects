using System;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        String CommandText { get; }

        /// <summary>
        /// Executes the specified session context.
        /// </summary>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        object Execute(SessionContext sessionContext);

        /// <summary>
        /// Prepares the operation.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <returns></returns>
        IOperation PrepareOperation(object entityModel);
    }
}