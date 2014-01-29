﻿using System.Data;

namespace VirtualObjects.Queries.Mapping
{
    /// <summary>
    /// Maps an entity based on the EntityInfo.
    /// Assumes that the order of the columns is the same as the result in the datareader.
    /// </summary>
    class OrderedEntityMapper : IEntityMapper
    {
        public virtual object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        {
            var i = 0;
            foreach ( var column in mapContext.EntityInfo.Columns )
            {
                column.SetFieldFinalValue(buffer, reader.GetValue(i++));
            }

            return buffer;
        }

        public virtual bool CanMapEntity(MapperContext context)
        {
            return context.EntityInfo != null && context.OutputType == context.EntityInfo.EntityType;
        }

        public virtual void PrepareMapper(MapperContext context)
        {
            // no prepare needed.
        }
    }
}