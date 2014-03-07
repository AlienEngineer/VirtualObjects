using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace VirtualObjects.Connections
{
    class Connection : IConnection, ITransaction
    {
        private readonly IDbConnectionProvider _provider;
        private readonly TextWriter _log;
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private bool _rolledBack;
        private bool _endedTransaction;
        private IDictionary<String, IOperationParameter> _stub = new Dictionary<String, IOperationParameter>();

        #region IDisposable Members
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposed )
            {
                if ( disposing )
                {
                    if ( _dbTransaction != null )
                    {
                        Commit();
                        _dbTransaction.Dispose();
                    }

                    _dbConnection.Dispose();
                }

                _dbConnection = null;
                _dbTransaction = null;

                _disposed = true;
            }
        }

        #endregion


        public Connection(IDbConnectionProvider provider, TextWriter log)
        {
            _provider = provider;
            _log = log;
            _dbConnection = provider.CreateConnection();
        }

        public IDbConnection DbConnection
        {
            get { return _dbConnection; }
        }

        public bool KeepAlive { get; set; }

        private TResult AutoClose<TResult>(Func<TResult> execute)
        {
            var value = execute();

            return value;
        }

        public object ExecuteScalar(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            return AutoClose(() => CreateCommand(commandText, parameters).ExecuteScalar());
        }

        public IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            return AutoClose(() => CreateCommand(commandText, parameters).ExecuteReader());
        }

        public int ExecuteNonQuery(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            return AutoClose(() => CreateCommand(commandText, parameters).ExecuteNonQuery());
        }

        public ITransaction BeginTransaction()
        {
            if ( _dbTransaction != null )
            {
                return new InnerTransaction(this);
            }

            Open();
            _dbTransaction = DbConnection.BeginTransaction();
            _endedTransaction = false;

            return this;
        }

        private void Open()
        {
            if ( _dbConnection == null )
            {
                _dbConnection = _provider.CreateConnection();
            }

            if ( _dbConnection.State == ConnectionState.Open )
            {
                return;
            }

            _dbConnection.Open();
            _log.WriteLine(Resources.Connection_Opened);
        }

        public void Close()
        {
            if ( !KeepAlive && !_endedTransaction || _dbConnection == null || _dbConnection.State == ConnectionState.Closed )
            {
                return;
            }

            _dbConnection.Close();
            _rolledBack = false;
            _dbTransaction = null;
            _endedTransaction = true;
            _log.WriteLine(Resources.Connection_Closed);
        }

        private IDbCommand CreateCommand(String commandText, IEnumerable<KeyValuePair<string, IOperationParameter>> parameters)
        {
            Open();
            var cmd = DbConnection.CreateCommand();

            cmd.Transaction = _dbTransaction;
            cmd.CommandText = commandText;

            (parameters ?? _stub)
                .Select(e => new { OperParameter = e, Parameter = cmd.CreateParameter() })
                .ForEach(e =>
                {
                    e.Parameter.ParameterName = e.OperParameter.Key;
                    e.Parameter.Value = e.OperParameter.Value.Value ?? DBNull.Value;

                    if ( e.OperParameter.Value.Type == typeof(Byte[]) )
                    {
                        e.Parameter.DbType = DbType.Binary;
                    }

                    cmd.Parameters.Add(e.Parameter);
                });

            _log.WriteLine(commandText);
            // Debug use only.
            // PrintCommand(cmd);

            return cmd;
        }


        [Conditional("DEBUG")]
        private void PrintCommand(IDbCommand cmd)
        {
            Trace.WriteLine("Command executed :\n");
            Trace.WriteLine("");

            foreach ( SqlParameter parameter in cmd.Parameters )
            {
                Trace.Write("Declare @" + parameter.ParameterName + " as " + parameter.SqlDbType);
                if ( parameter.Size > 0 )
                {
                    Trace.Write("(" + parameter.Size + ")");
                }

                Trace.WriteLine("");

                Trace.Write("Set @" + parameter.ParameterName + " = " + parameter.Value + "");

                if ( parameter.Value == DBNull.Value )
                {
                    Trace.Write("NULL");
                }

                Trace.WriteLine("");
            }

            Trace.WriteLine("");
            Trace.WriteLine(cmd.CommandText);
        }


        public void Rollback()
        {
            if ( _rolledBack )
            {
                return;
            }

            _rolledBack = true;
            _endedTransaction = true;

            // Makes a safe rollback.
            //
            // When the transaction is aborted by a trigger the transaction is 
            // automaticaly rolledback. Therefore it's no longer possible to rollback again.
            //
            try
            {
                _dbTransaction.Rollback();
                Close();
            }
            catch ( Exception ex )
            {
                Trace.WriteLine(ex.Message);
            }

        }

        public void Commit()
        {
            if ( _rolledBack || _endedTransaction )
            {
                return;
            }

            _endedTransaction = true;
            _dbTransaction.Commit();
        }

    }

}
