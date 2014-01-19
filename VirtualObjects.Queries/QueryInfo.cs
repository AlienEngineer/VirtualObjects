using System.Data.Common;

namespace VirtualObjects.Queries
{
    public interface IQueryInfo
    {
        DbCommand Command { get; }
    }

    class QueryInfo : IQueryInfo
    {
        public DbCommand Command { get; set; }
    }
}