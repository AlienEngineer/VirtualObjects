using System;
using System.Data;

namespace VirtualObjects
{
    public interface ITransaction : IDisposable
    {
        IDbConnection DbConnection { get; }
        void Rollback();
        void Commit();
    }
}