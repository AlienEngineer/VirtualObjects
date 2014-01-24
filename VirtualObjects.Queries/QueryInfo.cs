using System.Collections.Generic;
using System.Data.Common;

namespace VirtualObjects.Queries
{
    class QueryInfo : IQueryInfo
    {

        #region IQueryInfo Members
        
        public DbCommand Command { get; set; }

        public string CommandText { get; set; }
        
        public IDictionary<string, object> Parameters { get; set; }

        #endregion
    }
}