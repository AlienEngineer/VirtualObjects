using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VirtualObjects.CRUD;
using VirtualObjects.Programability;

namespace VirtualObjects.Connections
{
    class Connection : IConnection, ITransaction
    {
        private static int count = 0;
        
        
        private readonly IDbConnectionProvider _provider;
        private readonly TextWriter _log;
        private readonly IProgramability _programability;
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private bool _rolledBack;
        private bool _endedTransaction;
        private readonly IDictionary<String, IOperationParameter> _stub = new Dictionary<String, IOperationParameter>();

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
                    }

                    _dbConnection.Dispose();
                }

                _dbConnection = null;
                _dbTransaction = null;

                _disposed = true;
            }
        }

        #endregion


        public Connection(IDbConnectionProvider provider, TextWriter log, IProgramability programability)
        {
            _provider = provider;
            _log = log;
            _programability = programability;
            _dbConnection = provider.CreateConnection();
            ++count;
        }

        public IDbConnection DbConnection
        {
            get { return _dbConnection; }
        }

        public bool Rolledback { get; private set; }

        public bool KeepAlive { 
            get; 
            set; 
        }

        private TResult AutoClose<TResult>(Func<TResult> execute)
        {
            var value = execute();
            Close();
            return value;
        }

        public object ExecuteScalar(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            return AutoClose(() => CreateCommand(commandText, parameters).ExecuteScalar());
        }

        public object ExecuteScalar(IDbCommand cmd, IDictionary<string, IOperationParameter> parameters)
        {

            return AutoClose(() =>
            {
                RefreshParameters(cmd, parameters);

                cmd.Transaction = _dbTransaction;

                return cmd.ExecuteScalar();
            });
        }

#if DEBUG
        readonly Stack<String> commands = new Stack<String>();
        readonly Stack<IDataReader> readers = new Stack<IDataReader>();
#endif

        private IDataReader currentReader;

        public IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters)
        {

#if DEBUG
            commands.Push(commandText);
#endif

             //
             // This is not closed because the reader has to be closed by who ever is using it.
             // e.g. after mapping...
             //
            currentReader = CreateCommand(commandText, parameters).ExecuteReader();
#if DEBUG
            readers.Push(currentReader);
#endif
            return currentReader;
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
            _endedTransaction = true;
            _log.WriteLine(Resources.Connection_Opened);
        }

        public void Close()
        {
            if (currentReader != null && !currentReader.IsClosed)
            {
                currentReader.Close();
            }

            if ( KeepAlive )
            {
                return;
            }

            if ( !_endedTransaction || _dbConnection == null || _dbConnection.State == ConnectionState.Closed )
            {
                return;
            }

            _dbConnection.Close();
            _rolledBack = false;
            _dbTransaction = null;
            _endedTransaction = true;
            _log.WriteLine(Resources.Connection_Closed);
        }

        public int ExecuteProcedure(string storeProcedure, IEnumerable<KeyValuePair<string, object>> args)
        {
            var cmd = CreateCommand(storeProcedure);
            cmd.CommandType = CommandType.StoredProcedure;

            RefreshParameters(cmd, args
                .Select(e => new KeyValuePair<string, IOperationParameter>(
                    e.Key,
                    new OperationParameter
                    {
                        Name = e.Key,
                        Value = e.Value
                    }
                )));

            return cmd.ExecuteNonQuery();
        }

        public IDbCommand CreateCommand(string commandText)
        {
            Open();
            var cmd =  DbConnection.CreateCommand();
            cmd.Transaction = _dbTransaction;
            cmd.CommandText = commandText;

            return cmd;
        }

        public IDbCommand CreateCommand(String commandText, IEnumerable<KeyValuePair<string, IOperationParameter>> parameters)
        {
            var cmd =  CreateCommand(commandText);
            
            RefreshParameters(cmd, parameters);

            _log.WriteLine(commandText);
            // Debug use only.
            // PrintCommand(cmd);

            return cmd;
        }


        private void RefreshParameters(IDbCommand cmd, IEnumerable<KeyValuePair<string, IOperationParameter>> parameters)
        {
            cmd.Parameters.Clear();

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
            if ( _rolledBack || _endedTransaction || DbConnection.State != ConnectionState.Open )
            {
                return;
            }

            Rolledback = _rolledBack = true;
            _endedTransaction = true;

            _dbTransaction.Rollback();
            Close();
        }

        public void Commit()
        {
            if ( _rolledBack || _endedTransaction )
            {
                return;
            }

            _endedTransaction = true;
            _programability.ReleaseLock(this);
            _dbTransaction.Commit();
            Close();
        }

        public void AcquireLock(string resouceName)
        {
            _programability.AcquireLock(this, resouceName);
        }
    }

}
