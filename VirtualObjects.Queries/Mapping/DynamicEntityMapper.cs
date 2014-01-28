using System.Collections;
using System.Data;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.Queries.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    class DynamicEntityMapper : OrderedEntityMapper
    {
        public override object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        {
            var offset = 0;
            var propertyCount = 0;

            while (propertyCount < mapContext.Contexts.Count)
            {
                var ctx = mapContext.Contexts[propertyCount];
                var memberBuffer = mapContext.PropertyGetters[propertyCount](buffer);

                base.MapEntity(new OffsetedReader(reader, offset), memberBuffer, ctx);

                offset += ctx.EntityInfo.Columns.Count;
                propertyCount++;
            }

            return buffer;
        }

        public override bool CanMapEntity(MapperContext context)
        {
            var outputType = context.OutputType;
            var properties = outputType.Properties();

            return outputType.IsDynamic() &&
                   !properties.Any(e => e.PropertyType.InheritsOrImplements<IEnumerable>()) &&
                   properties.Any(e => !e.PropertyType.IsFrameworkType());
        }

        public override void PrepareMapper(MapperContext context)
        {
            base.PrepareMapper(context);

            var outputType = context.OutputType;

            context.PropertyGetters = outputType.Properties()
                .Select(e => e.DelegateForGetPropertyValue())
                .ToList();

            context.Contexts = outputType.Properties()
                .Select(e => new MapperContext
                {
                    EntityInfo = context.Mapper.Map(e.PropertyType),
                    OutputType = e.PropertyType,
                    EntityProvider = context.EntityProvider,
                    Mapper = context.Mapper
                })
                .ToList();


        }
    }
}