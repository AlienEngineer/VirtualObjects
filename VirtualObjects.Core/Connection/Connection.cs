using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace VirtualObjects.Core.Connection
{
    class Connection : IConnection, ITransaction
    {
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private bool _rolledBack;
        private bool _endedTransaction;


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
                    _dbConnection.Dispose();
                    if (_dbTransaction != null)
                    {
                        Commit();
                        _dbTransaction.Dispose();
                    }
                }

                _dbConnection = null;
                _dbTransaction = null;

                _disposed = true;
            }
        }

        #endregion


        public Connection(IDbConnectionProvider provider)
        {
            _dbConnection = provider.CreateConnection();
        }

        public IDbConnection DbConnection
        {
            get { return _dbConnection; }
        }

        public object ExecuteScalar(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            return CreateCommand(commandText, parameters).ExecuteScalar();
        }

        public IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            return CreateCommand(commandText, parameters).ExecuteReader();
        }

        public void ExecuteNonQuery(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            CreateCommand(commandText, parameters).ExecuteNonQuery();
        }

        public ITransaction BeginTranslation()
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
            if ( _dbConnection.State == ConnectionState.Open )
            {
                return;
            }

            _dbConnection.Open();
        }

        public void Close()
        {
            if ( !_endedTransaction || _dbConnection.State == ConnectionState.Closed )
            {
                return;
            }

            _dbConnection.Close();
            _rolledBack = false;
            _dbTransaction = null;
            _endedTransaction = true;
        }

        private IDbCommand CreateCommand(String commandText, IEnumerable<KeyValuePair<string, IOperationParameter>> parameters)
        {
            Open();
            var cmd = DbConnection.CreateCommand();

            cmd.Transaction = _dbTransaction;
            cmd.CommandText = commandText;

            parameters
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

            return cmd;
        }

        public void Rollback()
        {
            if ( _rolledBack )
            {
                return;
            }

            // Makes a safe rollback.
            //
            // When the transaction is aborted by a trigger the transaction is 
            // automaticaly rolledback. Therefore it's no longer possible to rollback again.
            //
            try
            {
                _dbTransaction.Rollback();
            }
            catch ( Exception ex )
            {
                Trace.WriteLine(ex.Message);
            }

            _rolledBack = true;
            _endedTransaction = true;
        }

        public void Commit()
        {
            if ( _rolledBack || _endedTransaction )
            {
                return;
            }

            _dbTransaction.Commit();
            _endedTransaction = true;
        }

    }

}
