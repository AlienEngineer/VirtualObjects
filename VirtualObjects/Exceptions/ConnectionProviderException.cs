using System;

namespace VirtualObjects.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionProviderException : VirtualObjectsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConnectionProviderException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public ConnectionProviderException(string message, object src) : base(message, src)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConnectionProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionProviderException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConnectionProviderException(string message, object src, Exception innerException) : base(message, src, innerException)
        {
        }
    }
}