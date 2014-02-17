using System;
using VirtualObjects.Exceptions;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public static class Error
    {

        /// <summary>
        /// Raises if true.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        /// <exception cref="VirtualObjects.Exceptions.VirtualObjectsException"></exception>
        public static void RaiseIfTrue(Func<Boolean> predicate, String message, Object src = null)
        {
            if (predicate())
                throw  new VirtualObjectsException(message, src);
        }

        /// <summary>
        /// Raises if null.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="message">The message.</param>
        /// <param name="src">The source.</param>
        public static void RaiseIfNull(this Object target, String message, Object src = null)
        {
            RaiseIfTrue(() => target == null, message, src);
        }
    }
}
