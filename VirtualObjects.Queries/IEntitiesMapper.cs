using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.Queries
{
    public interface IEntitiesMapper
    {
        IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader);
    }
}