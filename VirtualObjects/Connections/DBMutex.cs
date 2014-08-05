using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace VirtualObjects.Connections
{
    /// <summary>
    /// 
    /// </summary>
    class DBMutex : WaitHandle
    {
        private const int defaultTimeout = 40000;
        public string Name { get; private set; }
        private IDbConnection Connection { get; set; }
        private IDbTransaction Transaction { get; set; }

        public DBMutex(IDbConnection connection, string name)
        {
            Connection = connection;
            Name = name;
        }

        public override bool WaitOne()
        {
            return WaitOne(defaultTimeout);
        }

        public override bool WaitOne(int millisecondsTimeout)
        {
            return WaitOne(millisecondsTimeout, false);
        }
        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            return WaitOne(TimeSpan.FromMilliseconds(millisecondsTimeout), exitContext);
        }

        public override bool WaitOne(TimeSpan timeout)
        {
            return WaitOne(timeout, false);
        }

        public override bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            Connection.Open();
            var command = (SqlCommand)Connection.CreateCommand();
            Transaction = Connection.BeginTransaction();
            command.Connection = (SqlConnection)Connection;
            command.Transaction = (SqlTransaction)Transaction;
            command.Parameters.Add("@ErrorCode", SqlDbType.Int);
            command.Parameters["@ErrorCode"].Direction = ParameterDirection.ReturnValue;
            command.CommandText = String.Format(@"EXEC sp_getapplock                
            @Resource = '{0}',
            @LockMode = 'Exclusive',
            @LockOwner = 'Transaction',
            @LockTimeout = {1},
            @DbPrincipal = 'public'", Name, (long)timeout.TotalMilliseconds);
            command.ExecuteNonQuery();

            var result = command.Parameters["@ErrorCode"].Value;

            Console.WriteLine("Lock acquired with value: {0}", result);

            return true;
        }

        public void ReleaseDBMutex()
        {
            var command = Connection.CreateCommand();
            command.Connection = Connection;
            command.Transaction = Transaction;
            command.CommandText = String.Format(@"EXEC sp_releaseapplock
                        @Resource = '{0}',
                        @DbPrincipal = 'public',
                        @LockOwner = 'Transaction'", Name);
            command.ExecuteNonQuery();
            Transaction.Commit();
            Connection.Close();
        }

        protected override void Dispose(bool explicitDisposing)
        {
            if (Transaction != null)
                ReleaseDBMutex();
            Connection.Dispose();

            base.Dispose(explicitDisposing);
        }

    }
}
