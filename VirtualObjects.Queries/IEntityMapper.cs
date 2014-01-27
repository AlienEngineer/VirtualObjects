using System;
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
        object MapEntity(DbDataReader reader, object buffer, MapperContext mapContext);

    }

    public class MapperContext
    {
        public Type OutputType { get; set; }
        public IEntityInfo EntityInfo { get; set; }
    }
}
