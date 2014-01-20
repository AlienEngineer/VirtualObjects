using System;

namespace VirtualObjects
{
    public static class Error
    {

        public static void RaiseIfTrue(Func<Boolean> predicate, String message, Object src = null)
        {
            if (predicate())
                throw  new VirtualObjectsException(message, src);
        }

        public static void RaiseIfNull(this Object target, String message, Object src = null)
        {
            RaiseIfTrue(() => target == null, message, src);
        }
    }
}
