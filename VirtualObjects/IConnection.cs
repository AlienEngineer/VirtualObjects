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
        /// Creates the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        IDbCommand CreateCommand(string commandText);

        /// <summary>
        /// Reuses the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        IDbCommand ReuseCommand(IDbCommand command);

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
        /// Executes the reader.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters, out IDbCommand command);

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        IDataReader ExecuteReader(IDbCommand command, IDictionary<string, IOperationParameter> parameters);

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
        /// <param name="isolation"></param>
        /// <returns></returns>
        ITransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.Unspecified);
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
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        string ConnectionString { get; }

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

        /// <summary>
        /// Executes the procedure.
        /// </summary>
        /// <param name="storeProcedure">The store procedure.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        int ExecuteProcedure(string storeProcedure, IEnumerable<KeyValuePair<string, object>> args);
    }
}