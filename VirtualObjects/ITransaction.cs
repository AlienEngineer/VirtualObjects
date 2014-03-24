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
        Boolean Rolledback { get; }

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