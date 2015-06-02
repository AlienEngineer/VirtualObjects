using System;
using System.Collections.Generic;
using System.Data;
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

    /// <summary>
    /// 
    /// </summary>
    public class MapperContext
    {
        /// <summary>
        /// Gets or sets the type of the output.
        /// </summary>
        /// <value>
        /// The type of the output.
        /// </value>
        public Type OutputType { get; set; }

        /// <summary>
        /// Gets or sets the map entity.
        /// </summary>
        /// <value>
        /// The map entity.
        /// </value>
        public Func<object, object[], object> MapEntity { get; set; }

        /// <summary>
        /// Gets or sets the entity information.
        /// </summary>
        /// <value>
        /// The entity information.
        /// </value>
        public IEntityInfo EntityInfo { get; set; }
        /// <summary>
        /// Gets or sets the entity provider.
        /// </summary>
        /// <value>
        /// The entity provider.
        /// </value>
        public IEntityProvider EntityProvider { get; set; }
        /// <summary>
        /// Gets or sets the output type setters.
        /// </summary>
        /// <value>
        /// The output type setters.
        /// </value>
        public IList<MemberSetter> OutputTypeSetters { get; set; }

        /// <summary>
        /// Gets or sets the entity bag.
        /// </summary>
        /// <value>
        /// The entity bag.
        /// </value>
        public IEntityBag EntityBag { get; set; }
        /// <summary>
        /// Gets or sets the contexts.
        /// </summary>
        /// <value>
        /// The contexts.
        /// </value>
        public List<MapperContext> Contexts { get; set; }
        /// <summary>
        /// Gets or sets the property getters.
        /// </summary>
        /// <value>
        /// The property getters.
        /// </value>
        public List<MemberGetter> PropertyGetters { get; set; }
        /// <summary>
        /// Gets or sets the query information.
        /// </summary>
        /// <value>
        /// The query information.
        /// </value>
        public IQueryInfo QueryInfo { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [read].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read]; otherwise, <c>false</c>.
        /// </value>
        public bool Read { get; set; }
        /// <summary>
        /// Gets or sets the buffer.
        /// </summary>
        /// <value>
        /// The buffer.
        /// </value>
        public object Buffer { get; set; }

        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <returns></returns>
        public object CreateEntity()
        {
            return EntityProvider.CreateEntity(OutputType);
        }
    }
}
