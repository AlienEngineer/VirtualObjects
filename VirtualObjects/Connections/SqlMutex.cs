using System;
using System.Data;
using System.Threading;

namespace VirtualObjects.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlMutex : WaitHandle
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

        /// <summary>
        /// Blocks the current thread until the current <see cref="T:System.Threading.WaitHandle" /> receives a signal.
        /// </summary>
        /// <returns>
        /// true if the current instance receives a signal. If the current instance is never signaled, <see cref="M:System.Threading.WaitHandle.WaitOne(System.Int32,System.Boolean)" /> never returns.
        /// </returns>
        public override bool WaitOne()
        {
            return WaitOne(defaultTimeout);
        }

        /// <summary>
        /// Blocks the current thread until the current <see cref="T:System.Threading.WaitHandle" /> receives a signal, using a 32-bit signed integer to specify the time interval.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
        /// <returns>
        /// true if the current instance receives a signal; otherwise, false.
        /// </returns>
        public override bool WaitOne(int millisecondsTimeout)
        {
            return WaitOne(millisecondsTimeout, false);
        }

        /// <summary>
        /// Blocks the current thread until the current <see cref="T:System.Threading.WaitHandle" /> receives a signal, using a 32-bit signed integer to specify the time interval and specifying whether to exit the synchronization domain before the wait.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
        /// <param name="exitContext">true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, false.</param>
        /// <returns>
        /// true if the current instance receives a signal; otherwise, false.
        /// </returns>
        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            return WaitOne(TimeSpan.FromMilliseconds(millisecondsTimeout), exitContext);
        }

        /// <summary>
        /// Blocks the current thread until the current instance receives a signal, using a <see cref="T:System.TimeSpan" /> to specify the time interval.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
        /// <returns>
        /// true if the current instance receives a signal; otherwise, false.
        /// </returns>
        public override bool WaitOne(TimeSpan timeout)
        {
            return WaitOne(timeout, false);
        }

        /// <summary>
        /// Blocks the current thread until the current instance receives a signal, using a <see cref="T:System.TimeSpan" /> to specify the time interval and specifying whether to exit the synchronization domain before the wait.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="exitContext">true to exit the synchronization domain for the context before the wait (if in a synchronized context), and reacquire it afterward; otherwise, false.</param>
        /// <returns>
        /// true if the current instance receives a signal; otherwise, false.
        /// </returns>
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

        /// <summary>
        /// When overridden in a derived class, releases the unmanaged resources used by the <see cref="T:System.Threading.WaitHandle" />, and optionally releases the managed resources.
        /// </summary>
        /// <param name="explicitDisposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool explicitDisposing)
        {
            if (Transaction != null)
                ReleaseDBMutex();
            Connection.Dispose();

            base.Dispose(explicitDisposing);
        }

    }
}
