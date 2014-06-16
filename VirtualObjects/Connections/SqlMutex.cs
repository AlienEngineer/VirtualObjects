using System;
using System.Data;
using System.Threading;

namespace VirtualObjects.Connections
{
    /// <summary>
    /// 
    /// </summary>
    internal class SqlMutex : WaitHandle
    {
        private const int defaultTimeout = 40000;
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        private IDbConnection Connection { get; set; }
        private IDbTransaction Transaction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlMutex"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="dbTransaction"></param>
        /// <param name="name">The name.</param>
        public SqlMutex(IDbConnection connection, IDbTransaction dbTransaction, string name)
        {
            Connection = connection;
            Transaction = dbTransaction;
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
            var command = Connection.CreateCommand();
            command.Connection = Connection;
            command.Transaction = Transaction;
            command.CommandText = String.Format(@"EXEC sp_getapplock                
            @Resource = '{0}',
            @LockMode = 'Exclusive',
            @LockOwner = 'Transaction',
            @LockTimeout = {1},
            @DbPrincipal = 'public'", Name, (long)timeout.TotalMilliseconds);
            var result = command.ExecuteNonQuery();



            return true;
        }

        /// <summary>
        /// Releases the database mutex.
        /// </summary>
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
