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
            : base(AppendInnerExceptionToMessage(message, innerException), innerException)
        {
        }

        public VirtualObjectsException(String message, Object src, Exception innerException)
            : base(AppendInnerExceptionToMessage(message.FormatWith(src), innerException), innerException)
        {
        }

        private static String AppendInnerExceptionToMessage(String message, Exception innerException)
        {
            return innerException == null ? 
                message :
                string.Format("{0}\n{1}\n{2}", 
                    message, 
                    new string('=', 50),
                    innerException.Message);
        }
    }
}
