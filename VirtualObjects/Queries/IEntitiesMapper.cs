using System;
using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.Queries
{
    public interface IEntitiesMapper
    {
        IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader, IQueryInfo context);
        
        IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType);
    }
}