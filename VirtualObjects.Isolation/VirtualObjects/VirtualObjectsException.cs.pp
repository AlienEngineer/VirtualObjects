using System;

namespace $rootnamespace$
{
    public class VirtualObjectsException : Exception
    {
        public VirtualObjectsException(Exception innerException): base(innerException.Message, innerException) { }
		public VirtualObjectsException(String message): base(message) { }
    }
}
