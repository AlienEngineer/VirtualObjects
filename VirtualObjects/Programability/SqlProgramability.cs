using System;
using System.Collections.Generic;

namespace VirtualObjects.Programability
{
    class SqlProgramability : IProgramability
    {
        private readonly IList<string> _locks = new List<string>();

        public bool AcquireLock(IConnection connection, string resourceName, int timeout)
        {
            if (Acquire(connection, resourceName, timeout))
            {
                _locks.Add(resourceName);
                return true;
            }
            
            return false;
        }

        private static bool Acquire(IConnection connection, string resourceName, int timeout)
        {

            int result = connection.ExecuteProcedure("sp_getapplock", new[]
            {
                new KeyValuePair<string, object>("@Resource", resourceName),
                new KeyValuePair<string, object>("@LockMode", "Exclusive"),
                new KeyValuePair<string, object>("@LockTimeout", timeout),
                new KeyValuePair<string, object>("@DbPrincipal", "public"),
                new KeyValuePair<string, object>("@LockOwner", "Session")
            });

            Console.WriteLine("The lock acquire result was: {0}", result);

            return (result >= 0);
        }
        
        public void ReleaseLock(IConnection connection)
        {
            foreach (var @lock in _locks)
            {
                connection.ExecuteProcedure("sp_releaseapplock", new[]
                {
                    new KeyValuePair<string, object>("@Resource", @lock),
                    new KeyValuePair<string, object>("@LockOwner", "Session")
                });
            }

            _locks.Clear();
        }
    }
}