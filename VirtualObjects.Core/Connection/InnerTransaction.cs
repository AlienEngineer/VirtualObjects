using System.Data;

namespace VirtualObjects.Core.Connection
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

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Commit()
        {

        }

        public void Dispose()
        {
            Commit();
        }
    }
}