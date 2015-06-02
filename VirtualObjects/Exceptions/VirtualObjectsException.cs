using System;

namespace VirtualObjects.Exceptions
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class VirtualObjectsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public VirtualObjectsException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public VirtualObjectsException(string message, object src)
            : base(message.FormatWith(src))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public VirtualObjectsException(string message, Exception innerException)
            : base(AppendInnerExceptionToMessage(message, innerException), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        /// <param name="innerException">The inner exception.</param>
        public VirtualObjectsException(string message, object src, Exception innerException)
            : base(AppendInnerExceptionToMessage(message.FormatWith(src), innerException), innerException)
        {
        }


        private static string AppendInnerExceptionToMessage(string message, Exception innerException)
        {
            return innerException == null ? 
                message :
                string.Format("{0}\n{1}\n{2}", 
                    message, 
                    new string('=', 50),
                    innerException.Message);
        }
    }
}
