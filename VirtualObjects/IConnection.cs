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
        /// Gets a value indicating whether this instance is mars enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is mars enabled; otherwise, <c>false</c>.
        /// </value>
        Boolean IsMARSenabled { get; }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        IDbCommand CreateCommand(string commandText);

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
        /// Executes the scalar.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        object ExecuteScalar(IDbCommand cmd, IDictionary<string, IOperationParameter> parameters);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();
    }
}