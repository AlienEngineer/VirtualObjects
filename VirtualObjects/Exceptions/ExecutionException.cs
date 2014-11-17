using System;

namespace VirtualObjects.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ExecutionException : VirtualObjectsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ExecutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public ExecutionException(string message, object src) : base(message, src)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExecutionException(string message, object src, Exception innerException) : base(message, src, innerException)
        {
        }
    }
}