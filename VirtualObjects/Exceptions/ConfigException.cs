using System;

namespace VirtualObjects.Exceptions
{
    public class ConfigException : VirtualObjectsException
    {
        public ConfigException(string message) : base(message)
        {
        }

        public ConfigException(string message, object src) : base(message, src)
        {
        }

        public ConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConfigException(string message, object src, Exception innerException) : base(message, src, innerException)
        {
        }
    }
}