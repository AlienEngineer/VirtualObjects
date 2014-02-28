using System.Collections;
using System.Collections.Generic;
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
            mapContext.Buffer = buffer;

            while (propertyCount < mapContext.Contexts.Count)
            {
                var ctx = mapContext.Contexts[propertyCount];
                var memberBuffer = mapContext.PropertyGetters[propertyCount](buffer);

                base.MapEntity(new OffsetedReader(reader, offset), memberBuffer, ctx);

                offset += ctx.EntityInfo.Columns.Count;
                propertyCount++;
            }

            foreach ( var setter in mapContext.OutputTypeSetters )
            {
                setter(buffer, null);
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

            context.PropertyGetters = new List<MemberGetter>();
            context.Contexts = new List<MapperContext>();
            context.OutputTypeSetters = new List<MemberSetter>();

            var fields = outputType.Fields();

            fields.ForEach(field =>
            {
                context.PropertyGetters.Add(field.DelegateForGetFieldValue());

                var entityInfo = context.Mapper.Map(field.FieldType);

                context.Contexts.Add(new MapperContext
                {
                    EntityInfo = entityInfo,
                    OutputType = field.FieldType,
                    EntityProvider = context.EntityProvider,
                    Mapper = context.Mapper
                });

                if ( entityInfo == null )
                {
                    return;
                }

                entityInfo.ForeignKeys.ForEach(column =>
                {
                    var fieldInfo = fields.FirstOrDefault(e => column.Property.PropertyType == e.FieldType);
                    if (fieldInfo != null && fieldInfo.FieldType != field.FieldType)
                    {
                        var column1 = column;
                        context.OutputTypeSetters.Add((o, value) =>
                            MappingStatus.InternalLoad(() => column1.SetValue(field.Get(context.Buffer), fieldInfo.Get(context.Buffer)))
                        );
                    }
                });

            });
        }
    }
}