using System;
using System.Collections.Generic;
using System.Data.Common;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects.Queries
{
    class QueryInfo : IQueryInfo
    {

        #region IQueryInfo Members
        
        public DbCommand Command { get; set; }

        public string CommandText { get; set; }

        public IEntityInfo EntityInfo { get; set; }

        public IDictionary<string, IOperationParameter> Parameters { get; set; }

        public IList<IEntityColumnInfo> PredicatedColumns { get; set; }
        
        public Type OutputType { get; set; }
        
        internal QueryTranslator.CompilerBuffer Buffer { get; set; }

        public IEntitiesMapper EntitiesMapper { get; set; }

        #endregion
    }
}