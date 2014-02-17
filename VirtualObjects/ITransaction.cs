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
        /// Rollbacks the transaction.
        /// </summary>
        void Rollback();
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void Commit();
    }
}