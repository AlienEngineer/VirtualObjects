using System;

namespace VirtualObjects.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UnsupportedException : VirtualObjectsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnsupportedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public UnsupportedException(string message, object src)
            : base(message, src)
        {
        }
    }

}