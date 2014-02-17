using System;
using System.Collections.Generic;
using System.Data;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        object ExecuteScalar(string commandText, IDictionary<string, IOperationParameter> parameters);
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters);
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandText, IDictionary<string, IOperationParameter> parameters);

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        ITransaction BeginTransaction();
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <value>
        /// The database connection.
        /// </value>
        IDbConnection DbConnection { get; }
        /// <summary>
        /// Gets or sets a value indicating whether [keep alive].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [keep alive]; otherwise, <c>false</c>.
        /// </value>
        bool KeepAlive { get; set; }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();
    }
}