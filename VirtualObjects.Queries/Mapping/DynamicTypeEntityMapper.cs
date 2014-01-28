using System;
using System.Data;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.Queries.Mapping
{
    class DynamicTypeEntityMapper : IEntityMapper
    {
        class Data
        {
            public MemberSetter Setter { get; set; }
            public Object Value { get; set; }
        }

        public object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        {
            var i = 0;
            foreach ( var setter in mapContext.OutputTypeSetters )
            {
                setter(buffer, reader.GetValue(i++));
            }

            return buffer;
        }

        public bool CanMapEntity(MapperContext context)
        {
            var properties = context.OutputType.GetProperties();

            return context.OutputType.IsDynamic() &&
                properties.All(e => e.PropertyType.IsFrameworkType());
        }

        public void PrepareMapper(MapperContext context)
        {
            context.OutputTypeSetters = context.OutputType
                .Fields()
                .Select(e => e.DelegateForSetFieldValue())
                .ToList();
        }
    }
}