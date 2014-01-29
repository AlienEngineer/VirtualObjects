using System;
using System.Collections.Generic;
using System.Data.Common;
using VirtualObjects.Config;

namespace VirtualObjects.Queries
{
    class QueryInfo : IQueryInfo
    {

        #region IQueryInfo Members
        
        public DbCommand Command { get; set; }

        public string CommandText { get; set; }
        
        public IDictionary<string, object> Parameters { get; set; }

        public IList<IEntityColumnInfo> PredicatedColumns { get; set; }
        
        public Type OutputType { get; set; }

        #endregion
    }
}