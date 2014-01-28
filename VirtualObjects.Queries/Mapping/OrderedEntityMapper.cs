using System.Data;

namespace VirtualObjects.Queries.Mapping
{
    class OrderedEntityMapper : IEntityMapper
    {
        public object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        {
            var i = 0;
            foreach ( var column in mapContext.EntityInfo.Columns )
            {
                column.SetFieldFinalValue(buffer, reader.GetValue(i++));
            }

            return buffer;
        }

        public bool CanMapEntity(MapperContext context)
        {
            return context.EntityInfo != null && context.OutputType == context.EntityInfo.EntityType;
        }

        public void PrepareMapper(MapperContext context)
        {
            // no prepare needed.
        }
    }
}
