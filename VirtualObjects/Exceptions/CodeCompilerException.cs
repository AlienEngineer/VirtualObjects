using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualObjects.Exceptions
{
    public class CodeCompilerException : VirtualObjectsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompilerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CodeCompilerException(String message)
            : base(message)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompilerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public CodeCompilerException(String message, Object src)
            : base(message, src)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompilerException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CodeCompilerException(String message, Exception innerException)
            : base(message, innerException)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeCompilerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        /// <param name="innerException">The inner exception.</param>
        public CodeCompilerException(String message, Object src, Exception innerException)
            : base(message, src, innerException)
        {

        }
    }
}
