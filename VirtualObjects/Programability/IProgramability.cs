using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualObjects.Programability
{
    /// <summary>
    /// Layer responsible to handle StoreProcedures, functions and other programability aspects of the datasource.
    /// </summary>
    public interface IProgramability
    {
        /// <summary>
        /// Acquires the lock for the given resource.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="resouceName">Name of the resouce.</param>
        void AcquireLock(IConnection connection, string resouceName);

        /// <summary>
        /// Releases the lock for the given resource.
        /// </summary>
        /// <param name="connection">The connection.</param>
        void ReleaseLock(IConnection connection);
    }

    class SqlProgramability : IProgramability
    {
        private readonly IList<String> _locks = new List<string>();

        public void AcquireLock(IConnection connection, string resourceName)
        {
            _locks.Add(resourceName);

            connection.ExecuteProcedure("sp_getapplock", new[]
            {
                new KeyValuePair<string, object>("@Resource", resourceName),
                new KeyValuePair<string, object>("@LockMode", "Exclusive")
            });
        }

        public void ReleaseLock(IConnection connection)
        {
            foreach (var @lock in _locks)
            {
                connection.ExecuteProcedure("sp_releaseapplock", new[]
                {
                    new KeyValuePair<string, object>("@Resource", @lock)
                });
            }

            _locks.Clear();
        }
    }
}
