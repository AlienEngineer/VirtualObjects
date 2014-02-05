namespace VirtualObjects.Exceptions
{
    public class UnsupportedException : VirtualObjectsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnsupportedException(string message) : base(message)
        {
        }

        public UnsupportedException(string message, object src)
            : base(message, src)
        {
        }
    }

}