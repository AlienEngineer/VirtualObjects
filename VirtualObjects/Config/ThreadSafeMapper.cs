using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class ThreadSafeMapper : IMapper
    {
        private readonly IMapper mapper;
        private readonly IDictionary<Type, IEntityInfo> entityInfos = new Dictionary<Type, IEntityInfo>();
        
        public ThreadSafeMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// Maps the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEntityInfo Map(Type entityType)
        {
            lock ( entityInfos )
            {
                IEntityInfo entityInfo;

                if ( entityInfos.TryGetValue(entityType, out entityInfo) )
                {
                    return entityInfo;
                }

                entityInfo = mapper.Map(entityType);

                entityInfos[entityType] = entityInfo;

                return entityInfo;
            }
        }

        public void Dispose()
        {

        }
    }
}
