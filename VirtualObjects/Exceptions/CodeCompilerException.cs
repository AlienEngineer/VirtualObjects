using System;

namespace VirtualObjects.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CodeCompilerException : VirtualObjectsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompilerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CodeCompilerException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompilerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public CodeCompilerException(string message, object src)
            : base(message, src)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompilerException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CodeCompilerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompilerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        /// <param name="innerException">The inner exception.</param>
        public CodeCompilerException(string message, object src, Exception innerException)
            : base(message, src, innerException)
        {

        }
    }
}
