using System.Data.Common;

namespace VirtualObjects.Queries
{
    class QueryInfo : IQueryInfo
    {

        #region IQueryInfo Members
        
        public DbCommand Command { get; set; }

        public string CommandText { get; set; }
        
        #endregion
    }
}