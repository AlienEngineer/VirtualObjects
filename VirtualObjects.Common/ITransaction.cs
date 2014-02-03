using System.Data;

namespace VirtualObjects
{
    public interface ITransaction
    {
        IDbConnection DbConnection { get; }
        void Rollback();
        void Commit();
    }
}