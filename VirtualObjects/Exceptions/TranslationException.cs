using System;

namespace VirtualObjects.Exceptions
{
    public class TranslationException : VirtualObjectsException
    {
        public TranslationException(string message) : base(message)
        {
        }

        public TranslationException(string message, object src) : base(message, src)
        {
        }

        public TranslationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TranslationException(string message, object src, Exception innerException) : base(message, src, innerException)
        {
        }
    }
}