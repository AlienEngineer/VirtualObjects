using System.Collections.Generic;
using System.Data.Common;
using VirtualObjects.Config;

namespace VirtualObjects.Queries
{
    public interface IQueryInfo
    {
        string CommandText { get; }

        DbCommand Command { get; }
        
        IDictionary<string, object> Parameters { get; set; }
        IList<IEntityColumnInfo> PredicatedColumns { get; }
    }
}