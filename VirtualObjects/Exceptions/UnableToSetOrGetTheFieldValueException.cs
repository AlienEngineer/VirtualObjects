using System;

namespace VirtualObjects.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class UnableToSetOrGetTheFieldValueException : VirtualObjectsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToSetOrGetTheFieldValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnableToSetOrGetTheFieldValueException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToSetOrGetTheFieldValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public UnableToSetOrGetTheFieldValueException(string message, object src) : base(message, src)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToSetOrGetTheFieldValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnableToSetOrGetTheFieldValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToSetOrGetTheFieldValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnableToSetOrGetTheFieldValueException(string message, object src, Exception innerException)
            : base(message, src, innerException)
        {
        }
    }
}