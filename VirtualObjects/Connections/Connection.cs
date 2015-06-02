using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using VirtualObjects.CRUD;
using VirtualObjects.Programability;

namespace VirtualObjects.Connections
{
    class Connection : IConnection, ITransaction
    {
        private static int count;
        
        
        private readonly IDbConnectionProvider _provider;
        private readonly TextWriter _log;
        private readonly IProgramability _programmability;
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


        public Connection(IDbConnectionProvider provider, TextWriter log, IProgramability programmability)
        {
            _provider = provider;
            _log = log;
            _programmability = programmability;
            _dbConnection = provider.CreateConnection();
            ConnectionString = _dbConnection.ConnectionString;
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

        public string ConnectionString { get; private set; }

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

        public ITransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.Unspecified)
        {
            if ( _dbTransaction != null )
            {
                return new InnerTransaction(this);
            }

            Open();
            _dbTransaction = isolation == IsolationLevel.Unspecified ? 
                DbConnection.BeginTransaction() : 
                DbConnection.BeginTransaction(isolation);
            
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

            var param = cmd.CreateParameter();
            param.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();

            return (int) param.Value;
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

            //_log.WriteLine(commandText);
            // Debug use only.
            PrintCommand(cmd, _log);

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

        //[Conditional("DEBUG")]
        private void PrintCommand(IDbCommand cmd, TextWriter log)
        {
            log.WriteLine("Command executed :\n");
            log.WriteLine("");

            foreach ( IDbDataParameter parameter in cmd.Parameters )
            {
                log.Write("Declare @" + parameter.ParameterName + " as " + parameter.DbType);
                if ( parameter.Size > 0 )
                {
                    log.Write("(" + parameter.Size + ")");
                }

                log.WriteLine("");

                log.Write("Set @" + parameter.ParameterName + " = " + parameter.Value + "");

                if ( parameter.Value == DBNull.Value )
                {
                    log.Write("NULL");
                }

                log.WriteLine("");
            }

            log.WriteLine("");
            log.WriteLine(cmd.CommandText);
        }

        public void Rollback()
        {
            if ( _rolledBack || _endedTransaction || DbConnection.State != ConnectionState.Open )
            {
                return;
            }

            Rolledback = _rolledBack = true;
            _endedTransaction = true;

            _programmability.ReleaseLock(this);
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
            _programmability.ReleaseLock(this);
            _dbTransaction.Commit();
            Close();
        }

        public bool AcquireLock(string resourceName, int timeout = 30000)
        {
            return _programmability.AcquireLock(this, resourceName, timeout);
        }
    }

}
