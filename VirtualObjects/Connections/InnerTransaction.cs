using System;
using System.Data;

namespace VirtualObjects.Connections
{
    class InnerTransaction : ITransaction
    {
        private readonly ITransaction _transaction;

        public InnerTransaction(ITransaction transaction)
        {
            _transaction = transaction;
        }

        public IDbConnection DbConnection
        {
            get { return _transaction.DbConnection; }
        }

        public bool Rolledback { get; private set; }

        public void Rollback()
        {
            Rolledback = true;
            _transaction.Rollback();
        }

        public void Commit()
        {

        }

        public bool AcquireLock(string resouceName, int timeout)
        {
            return _transaction.AcquireLock(resouceName, timeout);
        }

        public void Dispose()
        {
            Commit();
        }
    }
}