using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Fasterflect;
using VirtualObjects.Config;

namespace VirtualObjects.Queries.Mapping
{
    class GroupedDynamicEntityMapper : DynamicWithMemberEntityMapper
    {
        private IList<IEntityInfo> entityInfos;

        public override bool CanMapEntity(MapperContext context)
        {
            var properties = context.OutputType.Fields();

            return context.OutputType.IsDynamic() &&
                   properties.Any(e => e.FieldType.InheritsOrImplements<IEnumerable>());
        }

        public override object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        {
            int i = 0;
            IEntityInfo entityInfo = null;

            foreach (var fieldInfo in mapContext.OutputType.Fields())
            {
                
                //
                // Handle collection type of field.
                //
                if (fieldInfo.FieldType.Implements<IEnumerable>())
                {
                    var collection = (IList)fieldInfo.Get(buffer);
                    int j;

                    var key = GetKey(reader, entityInfo);
                    do
                    {
                        j = i;
                        var entityType = mapContext.QueryInfo.PredicatedColumns[i].EntityInfo.EntityType;
                        var itemBuffer = mapContext.EntityProvider.CreateEntity(entityType);

                        while (fieldInfo.FieldType == mapContext.QueryInfo.PredicatedColumns[j].EntityInfo.EntityType)
                        {
                            mapContext.OutputTypeSetters[j](itemBuffer, reader.GetValue(j));
                            ++j;
                        }

                        collection.Add(itemBuffer);
                    
                    } while (reader.Read() && key == GetKey(reader, entityInfo));
                    
                    i = j;
                }

                //
                // Handle non collection type of field.
                //
                else
                {
                    entityInfo = mapContext.QueryInfo.PredicatedColumns[i].EntityInfo;
                    while(fieldInfo.FieldType == mapContext.QueryInfo.PredicatedColumns[i].EntityInfo.EntityType)
                    {
                        mapContext.OutputTypeSetters[i](buffer, reader.GetValue(i));
                        ++i;
                    }
                }
            }

            return buffer;
        }

        private int GetKey(IDataReader reader, IEntityInfo info)
        {
            return info.KeyColumns
                .Aggregate(new StringBuffer(), (current, keyColumn) => current + reader[keyColumn.ColumnName].ToString())
                .GetHashCode();
        }

        public override void PrepareMapper(MapperContext context)
        {
            var setters = new List<MemberSetter>();
            var predicatedCount = 0;
            var fieldCount = 0;

            context.OutputType.Fields().ForEach(e =>
            {
                var fieldType = e.FieldType;
                if (e.FieldType.InheritsOrImplements<IEnumerable>())
                {
                    fieldType = e.FieldType.GetGenericArguments().First();
                }

                if (fieldType.IsFrameworkType())
                {
                    setters.Add(e.DelegateForSetFieldValue());
                    fieldCount++;
                    return;
                }

                var ctx = new MapperContext
                {
                    EntityInfo = context.Mapper.Map(fieldType),
                    OutputType = fieldType,
                    EntityProvider = context.EntityProvider,
                    Mapper = context.Mapper,
                    QueryInfo = context.QueryInfo
                };

                var predictedColumn = ctx.QueryInfo.PredicatedColumns[predicatedCount];
                var type = predictedColumn.EntityInfo.EntityType;
                
                //
                // Created setters for each column of the same type.
                //
                while (predictedColumn.EntityInfo.EntityType == type)
                {
                    //
                    // Use the last bind because the value that comes from the database is not a complex type.
                    //
                    var column = predictedColumn.GetLastBind();

                    setters.Add((o, value) => column.SetFieldFinalValue(e.Get(o), value));

                    if (++predicatedCount == ctx.QueryInfo.PredicatedColumns.Count) break;

                    predictedColumn = ctx.QueryInfo.PredicatedColumns[predicatedCount];
                }

                fieldCount++;
            });

            context.OutputTypeSetters = setters;
        }
    }
}