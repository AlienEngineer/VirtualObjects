using System;
using System.Data;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <value>
        /// The database connection.
        /// </value>
        IDbConnection DbConnection { get; }

        /// <summary>
        /// Indicates that a rollback has taken place.
        /// </summary>
        /// <value>Has rooledback?</value>
        bool Rolledback { get; }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Acquires the lock.
        /// </summary>
        /// <param name="resouceName">Name of the resouce.</param>
        /// <param name="timeout"></param>
        /// <returns>[True] if lock acquired.</returns>
        bool AcquireLock(string resouceName, int timeout = 30000);
    }
}