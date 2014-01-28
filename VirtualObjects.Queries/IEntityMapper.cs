using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Fasterflect;
using VirtualObjects.Config;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// Map a reader row into a object
    /// </summary>
    public interface IEntityMapper
    {
        /// <summary>
        /// Maps the entity.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="mapContext">The map context.</param>
        /// <returns></returns>
        object MapEntity(IDataReader reader, object buffer, MapperContext mapContext);

        /// <summary>
        /// Determines whether this instance [can map entity] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        bool CanMapEntity(MapperContext context);

        /// <summary>
        /// Prepares the mapper.
        /// </summary>
        /// <param name="context">The context.</param>
        void PrepareMapper(MapperContext context);
    }

    public class MapperContext
    {
        public Type OutputType { get; set; }
        public IEntityInfo EntityInfo { get; set; }
        public IEntityProvider EntityProvider { get; set; }
        public IEnumerable<MemberSetter> OutputTypeSetters { get; set; }

        public object CreateEntity()
        {
            return EntityProvider.CreateEntity(OutputType);
        }
    }
}
