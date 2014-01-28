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

        /// <summary>
        /// Determines whether this instance can create the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        bool CanCreate(Type type);

        /// <summary>
        /// Gets the provider for the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IEntityProvider GetProviderForType(Type type);

        /// <summary>
        /// Gets or sets the main provider.
        /// </summary>
        /// <value>
        /// The main provider.
        /// </value>
        IEntityProvider MainProvider { get; set; }

        /// <summary>
        /// Prepares the provider.
        /// </summary>
        /// <param name="outputType">Type of the output.</param>
        void PrepareProvider(Type outputType);
    }
}
