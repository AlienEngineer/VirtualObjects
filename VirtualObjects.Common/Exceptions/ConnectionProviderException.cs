using System;

namespace VirtualObjects.Exceptions
{
    public class ConnectionProviderException : VirtualObjectsException
    {
        public ConnectionProviderException(string message) : base(message)
        {
        }

        public ConnectionProviderException(string message, object src) : base(message, src)
        {
        }

        public ConnectionProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConnectionProviderException(string message, object src, Exception innerException) : base(message, src, innerException)
        {
        }
    }
}