﻿using System;
using System.Data;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.Queries.Mapping
{
    class DynamicTypeEntityMapper : IEntityMapper
    {

        public virtual object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        {
            var i = 0;
            foreach ( var setter in mapContext.OutputTypeSetters )
            {
                setter(buffer, reader.GetValue(i++));
            }

            return buffer;
        }

        public virtual bool CanMapEntity(MapperContext context)
        {
            var properties = context.OutputType.Fields();

            return context.OutputType.IsDynamic() &&
                properties.All(e => e.FieldType.IsFrameworkType());
        }

        public virtual void PrepareMapper(MapperContext context)
        {
            context.OutputTypeSetters = context.OutputType
                .Fields()
                .Select(e => e.DelegateForSetFieldValue())
                .ToList();
        }
    }
}