using System;

namespace VirtualObjects.Exceptions
{
    public class UnableToSetOrGetTheFieldValueException : VirtualObjectsException
    {
        public UnableToSetOrGetTheFieldValueException(string message) : base(message)
        {
        }

        public UnableToSetOrGetTheFieldValueException(string message, object src) : base(message, src)
        {
        }

        public UnableToSetOrGetTheFieldValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnableToSetOrGetTheFieldValueException(string message, object src, Exception innerException)
            : base(message, src, innerException)
        {
        }
    }
}