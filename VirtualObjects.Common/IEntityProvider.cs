using System;

namespace VirtualObjects
{
    /// <summary>
    /// Provides a new instance of type.
    /// </summary>
    public interface IEntityProvider
    {

        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        Object CreateEntity(Type type);

        bool CanCreate(Type type);
        IEntityProvider GetProviderForType(Type type);
    }
}
