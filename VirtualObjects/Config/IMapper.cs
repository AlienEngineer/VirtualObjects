using System;

namespace VirtualObjects.Config
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMapper : IDisposable
    {
        /// <summary>
        /// Maps the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        IEntityInfo Map(Type entityType);
    }
}