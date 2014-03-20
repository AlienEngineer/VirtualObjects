using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntityBag
    {
        /// <summary>
        /// Gets or sets the <see cref="IEntityInfo"/> with the specified type.
        /// </summary>
        /// <value>
        /// The <see cref="IEntityInfo"/>.
        /// </value>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IEntityInfo this[Type type] { get; set; }
        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <returns></returns>
        bool TryGetValue(Type entityType, out IEntityInfo entityInfo);

    }

    class HybridEntityBag : IEntityBag
    {
        readonly IDictionary<Type, IEntityInfo> collection = new Dictionary<Type, IEntityInfo>();
        private readonly IEntityBag entityBag;

        public HybridEntityBag(IEntityBag entityBag)
        {
            this.entityBag = entityBag;
        }

        #region IEntityBag Members

        public IEntityInfo this[Type type]
        {
            get
            {
                IEntityInfo value = null;
                return TryGetValue(type, out value) ? value : null;
            }
            set
            {
                entityBag[type] = value;
                collection[type] = value;
            }
        }

        public bool TryGetValue(Type entityType, out IEntityInfo entityInfo)
        {
            if ( collection.TryGetValue(entityType, out entityInfo) ) return true;

            return entityBag.TryGetValue(entityType, out entityInfo);
        }

        #endregion
    }

    class EntityBag : IEntityBag
    {
        readonly ConcurrentDictionary<Type, IEntityInfo> collection = new ConcurrentDictionary<Type, IEntityInfo>();

        public bool TryGetValue(Type entityType, out IEntityInfo entityInfo)
        {
            return collection.TryGetValue(entityType, out entityInfo);
        }

        public IEntityInfo this[Type type]
        {
            get
            {
                IEntityInfo value = null;
                return TryGetValue(type, out value) ? value : null;
            }
            set { collection[type] = value; }
        }
    }
}
