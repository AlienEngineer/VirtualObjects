namespace VirtualObjects.Programability
{
    /// <summary>
    /// Layer responsible to handle StoreProcedures, functions and other programability aspects of the datasource.
    /// </summary>
    public interface IProgramability
    {
        /// <summary>
        /// Acquires the lock for the given resource.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="resouceName">Name of the resouce.</param>
        /// <param name="timeout">The timeout. Default: 30000</param>
        /// <returns>
        /// Returns [true] if the lock was acquired.
        /// </returns>
        bool AcquireLock(IConnection connection, string resouceName, int timeout);

        /// <summary>
        /// Releases the lock for the given resource.
        /// </summary>
        /// <param name="connection">The connection.</param>
        void ReleaseLock(IConnection connection);
    }
}
