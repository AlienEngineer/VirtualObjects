using System;
using System.Linq;

namespace VirtualObjects
{
    public class VirtualObjectsException : Exception
    {
        public VirtualObjectsException(String message) 
            : base(message)
        {
        }

        public VirtualObjectsException(String message, Object src)
            : base(message.FormatWith(src))
        {
        }

        public VirtualObjectsException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        public VirtualObjectsException(String message, Object src, Exception innerException)
            : base(message.FormatWith(src), innerException)
        {
        }
    }
}
