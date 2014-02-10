using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.Queries.Mapping
{

    class DynamicWithMemberEntityMapper : DynamicTypeEntityMapper
    {


        public override bool CanMapEntity(MapperContext context)
        {
            var properties = context.OutputType.Fields();

            return context.OutputType.IsDynamic() &&
                properties.Any(e => !e.FieldType.IsFrameworkType()) &&
                !properties.Any(e => e.FieldType.InheritsOrImplements<IEnumerable>() && e.FieldType != typeof(String)) &&
                properties.Any(e => e.FieldType.IsFrameworkType());
        }

        //public override object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        //{
        //    mapContext.Buffer = buffer;

        //    var i = 0;
        //    foreach ( var setter in mapContext.OutputTypeSetters )
        //    {
        //        setter(buffer, reader.GetValue(i++));
        //    }

        //    return buffer;
        //}


        public override void PrepareMapper(MapperContext context)
        {
            var setters = new List<MemberSetter>();
            var fieldCount = 0;
            var fields = context.OutputType.Fields();

            fields.ForEach(field =>
            {

                if ( field.FieldType.IsFrameworkType() )
                {
                    setters.Add(field.DelegateForSetFieldValue());
                    fieldCount++;
                    return;
                }

                var ctx = new MapperContext
                {
                    EntityInfo = context.Mapper.Map(field.FieldType),
                    OutputType = field.FieldType,
                    EntityProvider = context.EntityProvider,
                    Mapper = context.Mapper,
                    QueryInfo = context.QueryInfo
                };

                var predictedColumn = ctx.QueryInfo.PredicatedColumns[fieldCount];
                var type = predictedColumn.EntityInfo.EntityType;
                var i = fieldCount;


                //
                // Created setters for each column of the same type.
                //
                while ( predictedColumn.EntityInfo.EntityType == type )
                {

                    //
                    // Use the last bind because the value that comes from the database is not a complex type.
                    //
                    var column = predictedColumn.GetLastBind();

                    setters.Add((o, value) => column.SetFieldFinalValue(field.Get(o), value));

                    if ( ++i == ctx.QueryInfo.PredicatedColumns.Count ) break;

                    predictedColumn = ctx.QueryInfo.PredicatedColumns[i];
                }

                fieldCount++;
            });

            context.OutputTypeSetters = setters;
        }

    }
}