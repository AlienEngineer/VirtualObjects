using System;

namespace VirtualObjects.Exceptions
{

    /// <summary>
    /// 
    /// </summary>
    public class VirtualObjectsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public VirtualObjectsException(String message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public VirtualObjectsException(String message, Object src)
            : base(message.FormatWith(src))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public VirtualObjectsException(String message, Exception innerException)
            : base(AppendInnerExceptionToMessage(message, innerException), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        /// <param name="innerException">The inner exception.</param>
        public VirtualObjectsException(String message, Object src, Exception innerException)
            : base(AppendInnerExceptionToMessage(message.FormatWith(src), innerException), innerException)
        {
        }


        private static String AppendInnerExceptionToMessage(String message, Exception innerException)
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
