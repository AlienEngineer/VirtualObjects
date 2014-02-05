using System;

namespace VirtualObjects.Exceptions
{
    public class ExecutionException : VirtualObjectsException
    {
        public ExecutionException(string message) : base(message)
        {
        }

        public ExecutionException(string message, object src) : base(message, src)
        {
        }

        public ExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ExecutionException(string message, object src, Exception innerException) : base(message, src, innerException)
        {
        }
    }
}