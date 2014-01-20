using System.Data.Common;

namespace VirtualObjects.Queries
{
    public interface IQueryInfo
    {
        string CommandText { get; }

        DbCommand Command { get; }
    }
}