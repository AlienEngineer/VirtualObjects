using System;
using System.Data;
using System.Data.Common;
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

        bool CanMapEntity(MapperContext context);
    }

    public class MapperContext
    {
        public Type OutputType { get; set; }
        public IEntityInfo EntityInfo { get; set; }
        public IEntityProvider EntityProvider { get; set; }

        public object CreateEntity()
        {
            return EntityProvider.CreateEntity(OutputType);
        }
    }
}
