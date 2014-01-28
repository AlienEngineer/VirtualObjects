using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

            var column = mapContext.QueryInfo.PredicatedColumns.First();
            var entityInfo = column.EntityInfo;
            var key = GetKey(reader, entityInfo);

            FillEntity(reader, buffer, mapContext, column, entityInfo, 0);

            //
            // The key is the same to we need to fill the collection.
            //
            do
            {

            } while (key == GetKey(reader, entityInfo));

            return buffer;
        }

        private static void FillEntity(IDataReader reader, object buffer, MapperContext mapContext, IEntityColumnInfo column,
                                       IEntityInfo entityInfo, int i)
        {
            //
            // Fill the first object.
            //
            while (column.EntityInfo == entityInfo)
            {
                mapContext.OutputTypeSetters[i](buffer, reader.GetValue(i));

                column = mapContext.QueryInfo.PredicatedColumns[i];
                ++i;
            }
        }

        private int GetKey(IDataReader reader, IEntityInfo info)
        {
            return info.KeyColumns
                .Aggregate(new StringBuffer(), (current, keyColumn) => current + reader[keyColumn.ColumnName].ToString())
                .GetHashCode();
        }



        public override void PrepareMapper(MapperContext context)
        {
            base.PrepareMapper(context);

            
        }
    }
}