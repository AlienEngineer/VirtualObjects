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

        public void AcquireLock(string resouceName)
        {
            _transaction.AcquireLock(resouceName);
        }

        public void Dispose()
        {
            Commit();
        }
    }
}