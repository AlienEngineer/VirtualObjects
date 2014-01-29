using System;

namespace VirtualObjects.Exceptions
{
    public class MappingException : VirtualObjectsException
    {
        public MappingException(string message) : base(message)
        {
        }

        public MappingException(string message, object src) : base(message, src)
        {
        }

        public MappingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MappingException(string message, object src, Exception innerException) : base(message, src, innerException)
        {
        }
    }
}