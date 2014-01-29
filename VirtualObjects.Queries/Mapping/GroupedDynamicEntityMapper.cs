using System;
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
                   properties.Any(e => e.FieldType.InheritsOrImplements<IEnumerable>() && e.FieldType != typeof(String));
        }

        public override object MapEntity(IDataReader reader, object buffer, MapperContext mapContext)
        {
            var columns = mapContext.QueryInfo.PredicatedColumns;
            var fieldInfos = mapContext.OutputType.Fields();

            //
            // Handle non collection type of field.
            //
            var entityInfo = MapNonCollectionMembers(reader, buffer, mapContext, fieldInfos, columns);

            //
            // Handle collection type of field.
            //
            MapCollectionMembers(reader, buffer, mapContext, fieldInfos, columns, entityInfo);

            return buffer;
        }

        private static IEntityInfo MapNonCollectionMembers(
            IDataReader reader,
            object buffer,
            MapperContext mapContext,
            IEnumerable<FieldInfo> fieldInfos,
            IList<IEntityColumnInfo> columns)
        {
            IEntityInfo entityInfo = null;
            foreach ( var fieldInfo in fieldInfos.Where(e => !e.FieldType.Implements<IEnumerable>() || e.FieldType == typeof(String)) )
            {
                var i = FindColumnIndexOfType(fieldInfo.FieldType, 0, columns);

                entityInfo = columns[i].EntityInfo;
                while ( i < columns.Count && fieldInfo.FieldType == columns[i].EntityInfo.EntityType )
                {
                    mapContext.OutputTypeSetters[i](buffer, reader.GetValue(i));
                    ++i;
                }
            }

            return entityInfo;
        }

        private void MapCollectionMembers(
            IDataReader reader,
            object buffer,
            MapperContext mapContext,
            IEnumerable<FieldInfo> fieldInfos,
            IList<IEntityColumnInfo> columns,
            IEntityInfo entityInfo)
        {
            foreach ( var fieldInfo in fieldInfos.Where(e => e.FieldType.Implements<IEnumerable>() && e.FieldType != typeof(String)) )
            {
                var collection = (IList)fieldInfo.Get(buffer);
                var enumerableType = fieldInfo.FieldType.GetGenericArguments().First();

                var j = FindColumnIndexOfType(enumerableType, 0, columns);

                var key = GetKey(reader, entityInfo);
                do
                {
                    if ( key != GetKey(reader, entityInfo) )
                    {
                        break;
                    }

                    var entityType = columns[j].EntityInfo.EntityType;
                    var itemBuffer = mapContext.EntityProvider.CreateEntity(entityType);

                    while ( j < columns.Count && enumerableType == columns[j].EntityInfo.EntityType )
                    {
                        mapContext.OutputTypeSetters[j](itemBuffer, reader.GetValue(j));
                        ++j;
                    }

                    collection.Add(itemBuffer);


                    mapContext.Read = reader.Read();
                } while ( mapContext.Read );
            }
        }

        private static int FindColumnIndexOfType(Type type, int i, IList<IEntityColumnInfo> columns)
        {
            if ( i < columns.Count && columns[i].EntityInfo.EntityType == type )
            {
                return i;
            }

            for ( int j = 0; j < columns.Count; j++ )
            {
                if ( columns[j].EntityInfo.EntityType == type )
                {
                    return j;
                }
            }

            throw new MappingException("Something whent wrong.");
        }

        private int GetKey(IDataReader reader, IEntityInfo info)
        {
            return info.KeyColumns
                .Aggregate(new StringBuffer(), (current, keyColumn) => current + reader[keyColumn.ColumnName].ToString())
                .ToString()
                .GetHashCode();
        }

        public override void PrepareMapper(MapperContext context)
        {
            var setters = new List<MemberSetter>();
            var predicatedCount = 0;
            var fieldCount = 0;


            context.OutputType.Fields().ForEach(e =>
            {
                var collectionField = false;
                var fieldType = e.FieldType;
                if ( e.FieldType.InheritsOrImplements<IEnumerable>() && e.FieldType != typeof(String) )
                {
                    fieldType = e.FieldType.GetGenericArguments().First();
                    collectionField = true;
                }

                if ( fieldType.IsFrameworkType() )
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
                while ( predictedColumn.EntityInfo.EntityType == type )
                {
                    //
                    // Use the last bind because the value that comes from the database is not a complex type.
                    //
                    var column = predictedColumn;

                    if ( collectionField )
                    {
                        setters.Add((o, value) => column.SetFieldFinalValue(o, value));
                    }
                    else
                    {
                        setters.Add((o, value) => column.SetFieldFinalValue(e.Get(o), value));
                    }

                    if ( ++predicatedCount == ctx.QueryInfo.PredicatedColumns.Count ) break;

                    predictedColumn = ctx.QueryInfo.PredicatedColumns[predicatedCount];
                }

                fieldCount++;
            });

            context.OutputTypeSetters = setters;
        }
    }
}