using System.Collections.Generic;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.Queries.Mapping
{
    /// <summary>
    /// TODO: This type of mapping can be quite slow. A way to improve this is cache the entities as they are mapped, and reuse them.
    /// </summary>
    class DynamicWithMemberEntityMapper : DynamicTypeEntityMapper
    {

        public override bool CanMapEntity(MapperContext context)
        {
            var properties = context.OutputType.Fields();

            return context.OutputType.IsDynamic() &&
                   properties.Any(e => !e.FieldType.IsFrameworkType()) &&
                   properties.Any(e => e.FieldType.IsFrameworkType());
        }

        public override void PrepareMapper(MapperContext context)
        {
            var setters = new List<MemberSetter>();
            var fieldCount = 0;

            context.OutputType.Fields().ForEach(e =>
            {

                if ( e.FieldType.IsFrameworkType() )
                {
                    setters.Add(e.DelegateForSetFieldValue());
                    fieldCount++;
                    return;
                }

                var ctx = new MapperContext
                {
                    EntityInfo = context.Mapper.Map(e.FieldType),
                    OutputType = e.FieldType,
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
                while ( predictedColumn.EntityInfo.EntityType == type)
                {
                    //
                    // Use the last bind because the value that comes from the database is not a complex type.
                    //
                    var column = predictedColumn.GetLastBind();

                    setters.Add((o, value) => column.SetFieldFinalValue(e.Get(o), value));

                    if (++i == ctx.QueryInfo.PredicatedColumns.Count) break;

                    predictedColumn = ctx.QueryInfo.PredicatedColumns[i];
                }

                fieldCount++;
            });

            context.OutputTypeSetters = setters;
        }

    }
}